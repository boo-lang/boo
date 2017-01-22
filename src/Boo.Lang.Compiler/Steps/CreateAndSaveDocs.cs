#region license
// Copyright (c) 2014 by Harald Meyer auf'm Hofe (harald.meyer@users.sourceforge.net)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Generics;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// Event on XML documentation. Provides the designator
	/// of thee documented entity and the documentation itself
	/// as a string in XML form.
	/// </summary>
	public class DocEvent: EventArgs
	{
		public string Designator { get; private set; }
		public string Doc { get; private set; }
		
		public DocEvent(string designator, string doc)
		{
			this.Designator = designator;
			this.Doc = doc;
		}
	}
	
	/// <summary>
	/// Description of CreateAndSaveDocs.
	/// </summary>
	public class CreateAndSaveDocs : AbstractVisitorCompilerStep
	{
		public CreateAndSaveDocs(bool saveToFile)
		{
			this.SaveToFile = saveToFile;
		}
		
		public bool SaveToFile { get; private set; }
				
		public override void LeaveClassDefinition(Boo.Lang.Compiler.Ast.ClassDefinition node)
		{
			var designator=new StringBuilder();
			designator.Append("T:");
			designator.Append(node.FullName);
			this.ProcessDocumentation(designator.ToString(), node);
			base.LeaveClassDefinition(node);
		}
		
		public override void LeaveEnumDefinition(Boo.Lang.Compiler.Ast.EnumDefinition node)
		{
			var designator=new StringBuilder();
			designator.Append("T:");
			designator.Append(node.FullName);
			this.ProcessDocumentation(designator.ToString(), node);
			base.LeaveEnumDefinition(node);
		}
		
		public override void LeaveProperty(Boo.Lang.Compiler.Ast.Property node)
		{
			var designator=new StringBuilder();
			designator.Append("P:");
			designator.Append(node.FullName);
			this.ProcessDocumentation(designator.ToString(), node);
			base.LeaveProperty(node);
		}
		
		public override void LeaveField(Boo.Lang.Compiler.Ast.Field node)
		{
			var designator=new StringBuilder();
			designator.Append("F:");
			designator.Append(node.FullName);
			this.ProcessDocumentation(designator.ToString(), node);
			base.LeaveField(node);
		}
		
		/// <summary>
		/// Some collections of parameter type names.
		/// </summary>
		class GenericTypeParameters
		{
			/// <summary>
			/// List of parameter names introduced by a single backquote `
			/// </summary>
			public IGenericParameter[] FirstOrderParams { get; private set;}
			/// <summary>
			/// List of parameter names introduced by a double backquote ``++
			/// </summary>
			public IGenericParameter[] SecondOrderParams { get; private set;}
			
			public GenericTypeParameters()
			{
			}
			
			public GenericTypeParameters(IEntity anEntity)
			{
				var method = anEntity as IMethod;
				if (method != null)
					this.ReadMethodParams(method);
				else
				{
					var type = anEntity as IType;
					if (type != null)
						this.ReadTypeParams(type);
				}
			}
			
			public GenericTypeParameters(IType t)
			{
				this.ReadTypeParams(t);
			}
			public GenericTypeParameters(IMethod m)
			{
				this.ReadMethodParams(m);
			}
			
			void ReadTypeParams(IType t)
			{
				if (t.GenericInfo != null)
					this.FirstOrderParams = t.GenericInfo.GenericParameters;
			}
			
			void ReadMethodParams(IMethod m)
			{
				this.ReadTypeParams(m.DeclaringType);
				if (m.GenericInfo != null)
					this.SecondOrderParams = m.GenericInfo.GenericParameters;
			}
			
			public string ToString(string typeParamName)
			{
				if (this.FirstOrderParams != null)
				{
					foreach (var genericParam in this.FirstOrderParams)
						if (typeParamName.Equals(genericParam.Name))
							return "`"+genericParam.GenericParameterPosition.ToString();
				}
				else if (this.SecondOrderParams != null)
				{
					foreach (var genericParam in this.SecondOrderParams)
						if (typeParamName.Equals(genericParam.Name))
							return "``"+genericParam.GenericParameterPosition.ToString();					
				}
				return typeParamName;
			}
		}
		
		static int NumberOfGenericArguments(IType type)
		{
			if (type.GenericInfo != null)
				return type.GenericInfo.GenericParameters.Length;
			if (type.ConstructedInfo != null && type.ConstructedInfo.GenericArguments != null)
				return type.ConstructedInfo.GenericArguments.Length;
			return 0;
		}
		
		static string ToTypeName(string typeName)
		{
			switch (typeName)
			{
				case "byte": return typeof(byte).ToString();
				case "sbyte": return typeof(sbyte).ToString();
				case "short": return typeof(short).ToString();
				case "ushort": return typeof(ushort).ToString();
				case "int": return typeof(int).ToString();
				case "uint": return typeof(uint).ToString();
				case "long": return typeof(long).ToString();
				case "ulong": return typeof(ulong).ToString();
				case "single": return typeof(System.Single).ToString();
				case "double": return typeof(double).ToString();
				case "string": return typeof(string).ToString();
				case "object": return typeof(object).ToString();
				case "date": return typeof(DateTime).ToString();
				case "void": return typeof(void).ToString();
				case "timespan": return typeof(TimeSpan).ToString();
				default:
					return typeName;
			}
		}
		
		/// <summary>
		/// Returns the CIL designator of a particular type.
		/// </summary>
		/// <param name="tref">The reference to a type. Entity must have been resolved.</param>
		/// <returns></returns>
		static string CilTypeName(GenericTypeParameters generics, Ast.TypeReference typeRef)
		{
			return CilTypeName(generics, (IType)typeRef.Entity);
		}
		static string CilTypeName(GenericTypeParameters generics, IType typeEntity)
		{
			// TODO this method should also complete the qualifiers.
			if (GenericsServices.IsGenericParameter(typeEntity))
			{
				return generics.ToString(typeEntity.Name);
			}
			else
			{
				var result = ToTypeName( typeEntity.FullName );
				var numberOfGenericArguments=NumberOfGenericArguments(typeEntity);
				if (numberOfGenericArguments > 0)
				{
					result+="{";
					int iGenericArg=0;
					foreach(var genericArgument in typeEntity.ConstructedInfo.GenericArguments)
					{
						if (iGenericArg > 0)
							result+=",";
						result+=CilTypeName(generics, genericArgument);
						iGenericArg+=1;
					}
					result+="}";
				}
				return result;
			}
		}
		
		public override void LeaveMethod(Boo.Lang.Compiler.Ast.Method node)
		{
			var method = (IMethod) node.Entity;
			var designator=new StringBuilder();
			designator.Append("M:");
			var nodeName=node.FullName;
			var posGenericParams=nodeName.IndexOf('[');
			if (posGenericParams >= 0)
				nodeName=nodeName.Substring(0, posGenericParams);
			designator.Append(nodeName);
			if (GenericsServices.IsGenericMethod(node.Entity))
			{
				designator.Append("``"); // TODO: I will not deal with nested classes here. ` for the class parameters, `` for method parameters. That's it for now.
				designator.Append(GenericsServices
				                  .GetMethodGenerity(method)
				                  .ToString());
			}
			if (node.Parameters.Count > 0)
			{
				designator.Append("(");
				int i=0;
				var genericParams = new GenericTypeParameters(method);
				foreach (Ast.ParameterDeclaration p in node.Parameters)
				{
					if (i > 0) designator.Append(",");
					designator.Append(CilTypeName(genericParams, p.Type));
					i+=1;
				}
				designator.Append(")");
			}
			this.ProcessDocumentation(designator.ToString(), node);
			base.LeaveMethod(node);
		}
		
		System.Collections.Generic.Dictionary<string, Boo.Lang.Compiler.Ast.LexicalInfo> _knownNamespaces=new System.Collections.Generic.Dictionary<string, Boo.Lang.Compiler.Ast.LexicalInfo>();
		public override bool Visit(Boo.Lang.Compiler.Ast.Node node)
		{
			var nsDecl = node as Boo.Lang.Compiler.Ast.NamespaceDeclaration;
			if (nsDecl != null)
			{
				Boo.Lang.Compiler.Ast.LexicalInfo infoOnKnownNamespace;
				if (_knownNamespaces.TryGetValue(nsDecl.Name, out infoOnKnownNamespace))
					this.Warnings.Add(CompilerWarningFactory.CustomWarning(nsDecl.LexicalInfo, "This namespace has already been documented at "+infoOnKnownNamespace.ToString()+"."));
				else if (this.ProcessDocumentation("N:"+nsDecl.Name, nsDecl))
					_knownNamespaces.Add(nsDecl.Name, nsDecl.LexicalInfo);
			}
			return base.Visit(node);
		}
				
		public string DocFileName
		{
			get
			{
				if (!this.Parameters.CreateDoc)
					return null;
				if (string.IsNullOrEmpty(this.Parameters.OutputDoc))
					return
						Path.Combine(Path.GetDirectoryName(this.Parameters.OutputAssembly),
						             Path.GetFileNameWithoutExtension(this.Parameters.OutputAssembly)+".xml");
				else
					return this.Parameters.OutputDoc;
			}
		}
		
		public override void Run()
		{
			this.GetDestination();
			base.Run();
			if (this._destination != null)
			{
				this._destination.WriteEndElement(); // members
				this._destination.WriteEndElement(); // doc
				this._destination.Close();
				this._destination = null;
			}
		}

		public override void LeaveModule(Boo.Lang.Compiler.Ast.Module node)
		{
			base.LeaveModule(node);
		}		
		
		XmlTextWriter _destination;
		XmlWriter GetDestination()
		{
			if (_destination == null && this.Parameters.CreateDoc)
			{
				var filename=this.DocFileName;
				if (string.IsNullOrEmpty(filename)) return null;
				this._destination = new XmlTextWriter(filename, new System.Text.UTF8Encoding(false));
				this._destination.Formatting = Formatting.Indented;
				this._destination.WriteStartDocument();
				this._destination.WriteStartElement("doc");
				this._destination.WriteStartElement("assembly");
				this._destination.WriteElementString("name", Path.GetFileNameWithoutExtension(this.Parameters.OutputAssembly));
				this._destination.WriteEndElement();
				this._destination.WriteStartElement("members");
			}
			return _destination;
		}
		
		bool ProcessDocumentation(string designator, Ast.Node nodeWithDoc)
		{
			if (string.IsNullOrEmpty(nodeWithDoc.Documentation)) return false;
			var dest=this.GetDestination();
			if (dest != null)
			{
				dest.WriteStartElement("member");
				dest.WriteAttributeString("name", designator);
				WriteDoc(dest, nodeWithDoc);
				dest.WriteEndElement();
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// A sequence of string defining the tags in a Boo
		/// documentation. Its in fact a sequence of string pairs,
		/// where the first one is a regex and the second is the
		/// name of an XML tag that corresponds in a standard
		/// XML doc.
		/// The pattern defines the groups "name" or "cref" and
		/// "rest".
		/// </summary>
		readonly string[] _rawPatternTag=new String[]
		{
			@"Param\s+(?<name>(\w|\.)+):\s*(?<rest>.*)", "param",
			@"Parameter\s+(?<name>(\w|\.)+):\s*(?<rest>.*)", "param",
			@"Raises\s+(?<cref>(\w|\.)+):\s*(?<rest>.*)", "exception",
			@"Example:\s*(?<rest>.*)", "example",
			@"Permission\s+(?<cref>(\w|\.)+):\s*(?<rest>.*)", "permission",
			@"See:\s*(?<cref>.*)", "see",
			@"See\s+also:\s*(?<cref>.*)", "seealso",
		};
		
		/// <summary>
		/// A sequence of string pairs where the first string is
		/// the prefix of a line introducing a new section in a Boo
		/// documentation and the second string is the corresponding
		/// XML tag in a standard XML documentation.
		/// </summary>
		/// <remarks></remarks>
		readonly string[] _prefixTagPairs=
		{
			"Remarks:", "remarks",
			"Returns:", "returns",
		};
		
		ICollection<KeyValuePair<string, Regex>> _patternTagPairs;
		ICollection<KeyValuePair<string, Regex>> PatternTagPairs
		{
			get
			{
				if (this._patternTagPairs == null)
				{
					this._patternTagPairs = new List<KeyValuePair<string, Regex>>();
					for(int i = 0; i < this._rawPatternTag.Length; i+=2)
					{
						this._patternTagPairs.Add(new KeyValuePair<string, Regex>(
							this._rawPatternTag[i+1],
							new Regex(this._rawPatternTag[i], RegexOptions.IgnoreCase)));
					}
				}
				return this._patternTagPairs;
			}
		}
		
		
		string NormalizeCRef(Ast.Node scope, string origName)
		{
			// TODO: Will not mess around dereferencing types because I do not understand the corresponding code. This method only removes generic type parameters for now.
			var posGenericParams=origName.IndexOf('[');
			if (posGenericParams >= 0)
				return origName.Substring(0, posGenericParams);
			posGenericParams = origName.IndexOf(" of");
			if (posGenericParams >= 0)
				return origName.Substring(0, posGenericParams);			
			return origName;
		}
		
		readonly string[] _inTextTagPatterns=new String[]
		{
			@"\<(?<cref>(\w|\.)+)\>", "see",
			@"\[(?<name>(\w|\.)+)\]", "paramref",
		};
		ICollection<KeyValuePair<string, Regex>> _inTextTagPatternPairs;
		ICollection<KeyValuePair<string, Regex>> InTextTagPatternPairs
		{
			get
			{
				if (this._inTextTagPatternPairs == null)
				{
					this._inTextTagPatternPairs = new List<KeyValuePair<string, Regex>>();
					for(int i = 0; i < this._inTextTagPatterns.Length; i+=2)
					{
						this._inTextTagPatternPairs.Add(new KeyValuePair<string, Regex>(
							this._inTextTagPatterns[i+1],
							new Regex(this._inTextTagPatterns[i], RegexOptions.IgnoreCase)));
					}
				}
				return this._inTextTagPatternPairs;
			}
		}
				
		/// <summary>
		/// Normalize <paramref name="text"/>. Replace &lt..&gt; by
		/// "see" tag and [..] by "paramref" tag.
		/// </summary>
		void WriteText(XmlWriter dest, string text)
		{
			var allMatches=new SortedList<int, KeyValuePair<string, Match>>();
			foreach(var patternPair in this.InTextTagPatternPairs)
			{
				var matches=patternPair.Value.Matches(text);
				foreach(Match m in matches)
				{
					if (m.Success)
						allMatches[m.Index]=new KeyValuePair<string, Match>(patternPair.Key, m);
				}
			}
			int startPos=0;
			foreach(var tagPlusMatch in allMatches.Values)
			{
				var m = tagPlusMatch.Value;
				if (m.Index > startPos)
					dest.WriteString(text.Substring(startPos, m.Index-startPos));
				startPos=m.Index+m.Length;
				dest.WriteStartElement(tagPlusMatch.Key);
				var cref=m.Groups["cref"];
				var name=m.Groups["name"];
				if (name.Success)
				{
					dest.WriteAttributeString("name", name.Value);
				}
				else if (cref.Success)
				{
					dest.WriteAttributeString("cref", cref.Value);
				}
				dest.WriteEndElement();
			}
			if (startPos < text.Length)
				dest.WriteString(text.Substring(startPos, text.Length-startPos));
		}
		
		/// <summary>
		/// Produce a string in standard XML notation.
		/// </summary>
		/// <param name="doc">String documentation in Boo form.</param>
		void WriteDoc(XmlWriter dest, Ast.Node nodeWithDoc)
		{
			var doc=nodeWithDoc.Documentation.Replace("\r\n", "\n");
			dest.WriteStartElement("summary");
			var lineNoInParagraph=0;
			foreach(var lineOrig in doc.Split('\n'))
		    {
				var line = lineOrig.Trim();
				var done=false;
				for(int i=0; i < this._prefixTagPairs.Length; i+=2)
				{
					if (line.StartsWith(this._prefixTagPairs[i], StringComparison.InvariantCultureIgnoreCase))
					{
						dest.WriteEndElement();
						dest.WriteStartElement(this._prefixTagPairs[i+1]);
						if (line.Length > this._prefixTagPairs[i].Length)
							this.WriteText(dest, line.Substring(this._prefixTagPairs[i].Length));
						done=true;
						break;
					}
				}
				if (done)
					continue;
				foreach(var patternPair in this.PatternTagPairs)
				{
					var m=patternPair.Value.Match(line);
					if (m.Success)
					{
						var rest=m.Groups["rest"];
						var cref=m.Groups["cref"];
						var name=m.Groups["name"];
						if (name.Success)
						{
							dest.WriteEndElement();
							dest.WriteStartElement(patternPair.Key);
							dest.WriteAttributeString("name", name.ToString());
							if (rest.Success)
								this.WriteText(dest, rest.ToString());
							done=true;
							break;
						}
						else if (cref.Success)
						{
							dest.WriteEndElement();
							dest.WriteStartElement(patternPair.Key);
							dest.WriteAttributeString("cref", NormalizeCRef(nodeWithDoc, cref.ToString()));
							if (rest.Success)
								this.WriteText(dest, rest.ToString());
							done=true;
							break;
						}
					}
				}
				lineNoInParagraph+=1;
				if (done)
					continue;
				else
				{
					if (lineNoInParagraph > 1)
						dest.WriteString(" ");
					this.WriteText(dest, line);
				}
			}
			dest.WriteEndElement();
		}
	}
}
