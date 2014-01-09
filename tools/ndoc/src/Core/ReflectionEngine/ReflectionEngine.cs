// Copyright (C) 2001  Kral Ferch, Jason Diamond
// Parts Copyright (C) 2004  Kevin Downs
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.ComponentModel;

namespace NDoc.Core.Reflection
{
	/// <summary>
	/// Summary description for ReflectionEngine.
	/// </summary>
	public class ReflectionEngine : MarshalByRefObject
	{
		/// <summary>
		/// constructor for ReflectionEngine.
		/// </summary>
		public ReflectionEngine()
		{
		}

		ReflectionEngineParameters rep;
					
		AssemblyLoader			assemblyLoader;

		AssemblyXmlDocCache		assemblyDocCache;
		ExternalXmlSummaryCache	externalSummaryCache;
		Hashtable notEmptyNamespaces;
		Hashtable documentedTypes;

		ImplementsCollection implementations;
		TypeHierarchy derivedTypes;
		TypeHierarchy interfaceImplementingTypes;
		NamespaceHierarchyCollection namespaceHierarchies;
		TypeHierarchy baseInterfaces;
		AttributeUsageDisplayFilter attributeFilter;


		/// <summary>
		/// Gets the namespaces from assembly.
		/// </summary>
		/// <param name="rep">ReflectionEngine Parameters.</param>
		/// <param name="assemblyFile">Assembly file name.</param>
		/// <returns></returns>
		public SortedList GetNamespacesFromAssembly(ReflectionEngineParameters rep, string assemblyFile)
		{
			this.rep = rep;
			assemblyLoader = SetupAssemblyLoader();
			try
			{
				Assembly a = assemblyLoader.LoadAssembly(assemblyFile);
				SortedList namespaces = new SortedList();

				foreach (Type t in a.GetTypes())
				{
					string ns = t.Namespace;
				{
					if (ns == null)
					{
						if ((!namespaces.ContainsKey("(global)")))
							namespaces.Add("(global)", null);
					}
					else
					{
						if ((!namespaces.ContainsKey(ns)))
							namespaces.Add(ns, null);
					}
				}
				}

				return namespaces;
			}
			catch (ReflectionTypeLoadException rtle)
			{
				StringBuilder sb = new StringBuilder();
				if (assemblyLoader.UnresolvedAssemblies.Count > 0)
				{
					sb.Append("One or more required assemblies could not be located : \n");
					foreach (string ass in assemblyLoader.UnresolvedAssemblies)
					{
						sb.AppendFormat("   {0}\n", ass);
					}
					sb.Append("\nThe following directories were searched, \n");
					foreach (string dir in assemblyLoader.SearchedDirectories)
					{
						sb.AppendFormat("   {0}\n", dir);
					}
				}
				else
				{
					Hashtable fileLoadExceptions = new Hashtable();
					foreach (Exception loaderEx in rtle.LoaderExceptions)
					{
						System.IO.FileLoadException fileLoadEx = loaderEx as System.IO.FileLoadException;
						if (fileLoadEx != null)
						{
							if (!fileLoadExceptions.ContainsKey(fileLoadEx.FileName))
							{
								fileLoadExceptions.Add(fileLoadEx.FileName, null);
								sb.Append("Unable to load: " + fileLoadEx.FileName + "\r\n");
							}
						}
						sb.Append(loaderEx.Message + Environment.NewLine);
						sb.Append(loaderEx.StackTrace + Environment.NewLine);
						sb.Append("--------------------" + Environment.NewLine + Environment.NewLine);
					}
				}
				throw new DocumenterException(sb.ToString());
			}
			finally
			{
				assemblyLoader.Deinstall();
			}
		}

		/// <summary>Builds an Xml file combining the reflected metadata with the /doc comments.</summary>
		/// <returns>full pathname of XML file</returns>
		/// <remarks>The caller is responsible for deleting the xml file after use...</remarks>
		internal void MakeXmlFile(ReflectionEngineParameters rep, string xmlFile)
		{
			this.rep = rep;

			XmlTextWriter writer = null;
			try
			{
				writer = new XmlTextWriter(xmlFile, Encoding.UTF8);
				BuildXml(writer);
			}
			finally
			{
				if (writer != null)  writer.Close();
			}			
		}


		/// <summary>Builds an Xml string combining the reflected metadata with the /doc comments.</summary>
		/// <remarks>This now evidently writes the string in utf-16 format (and 
		/// says so, correctly I suppose, in the xml text) so if you write this string to a file with 
		/// utf-8 encoding it will be unparseable because the file will claim to be utf-16
		/// but will actually be utf-8.</remarks>
		/// <returns>XML string</returns>
		internal string MakeXml(ReflectionEngineParameters rep)
		{
			this.rep = rep;

			StringWriter swriter = new StringWriter();
			XmlWriter writer = new XmlTextWriter(swriter);

			try
			{
				BuildXml(writer);
				return swriter.ToString();
			}
			finally
			{
				if (writer != null)  writer.Close();
				if (swriter != null) swriter.Close();
			}

		}

		/// <summary>Builds an Xml file combining the reflected metadata with the /doc comments.</summary>
		private void BuildXml(XmlWriter writer)
		{
			int start = Environment.TickCount;

			Debug.WriteLine("Memory making xml: " + GC.GetTotalMemory(false).ToString());

			try
			{
				assemblyLoader = SetupAssemblyLoader();

				string DocLangCode = Enum.GetName(typeof(SdkLanguage), this.rep.SdkDocLanguage).Replace("_", "-");
				externalSummaryCache = new ExternalXmlSummaryCache(DocLangCode);

				notEmptyNamespaces = new Hashtable();

				namespaceHierarchies = new NamespaceHierarchyCollection();
				baseInterfaces = new TypeHierarchy();
				derivedTypes = new TypeHierarchy();
				interfaceImplementingTypes = new TypeHierarchy();
				attributeFilter = new AttributeUsageDisplayFilter(this.rep.DocumentedAttributes);
			
				documentedTypes = new Hashtable();
			
				PreReflectionProcess();

				string currentAssemblyFilename = "";

				try
				{
					// Start the document with the XML declaration tag
					writer.WriteStartDocument();

					// Start the root element
					writer.WriteStartElement("ndoc");
					writer.WriteAttributeString("SchemaVersion", "1.4");

					if (this.rep.FeedbackEmailAddress.Length > 0)
						WriteFeedBackEmailAddress(writer);

					if (this.rep.CopyrightText.Length > 0)
						WriteCopyright(writer);

					if (this.rep.IncludeDefaultThreadSafety)
						WriteDefaultThreadSafety(writer);

					if (this.rep.Preliminary)
						writer.WriteElementString("preliminary", "");

					WriteNamespaceHierarchies(writer);

					foreach (string AssemblyFileName in this.rep.AssemblyFileNames)
					{
						currentAssemblyFilename = AssemblyFileName;
						Assembly assembly = assemblyLoader.LoadAssembly(currentAssemblyFilename);

						int starta = Environment.TickCount;

						WriteAssembly(writer, assembly);

						Trace.WriteLine("Completed " + assembly.FullName);
						Trace.WriteLine(((Environment.TickCount - starta) / 1000.0).ToString() + " sec.");
					}

					writer.WriteEndElement();
					writer.WriteEndDocument();
					writer.Flush();

					Trace.WriteLine("MakeXML : " + ((Environment.TickCount - start) / 1000.0).ToString() + " sec.");

					// if you want to see NDoc's intermediate XML file, use the XML documenter.
				}
				finally
				{
					if (assemblyLoader != null)
					{
						assemblyLoader.Deinstall();
					}
				}
			}
			catch (ReflectionTypeLoadException rtle)
			{
				StringBuilder sb = new StringBuilder();
				if (assemblyLoader.UnresolvedAssemblies.Count > 0)
				{
					sb.Append("One or more required assemblies could not be located : \n");
					foreach (string ass in assemblyLoader.UnresolvedAssemblies)
					{
						sb.AppendFormat("   {0}\n", ass);
					}
					sb.Append("\nThe following directories were searched, \n");
					foreach (string dir in assemblyLoader.SearchedDirectories)
					{
						sb.AppendFormat("   {0}\n", dir);
					}
				}
				else
				{
					Hashtable fileLoadExceptions = new Hashtable();
					foreach (Exception loaderEx in rtle.LoaderExceptions)
					{
						System.IO.FileLoadException fileLoadEx = loaderEx as System.IO.FileLoadException;
						if (fileLoadEx != null)
						{
							if (!fileLoadExceptions.ContainsKey(fileLoadEx.FileName))
							{
								fileLoadExceptions.Add(fileLoadEx.FileName, null);
								sb.Append("Unable to load: " + fileLoadEx.FileName + "\r\n");
							}
						}
						sb.Append(loaderEx.Message + Environment.NewLine);
						sb.Append(loaderEx.StackTrace + Environment.NewLine);
						sb.Append("--------------------" + Environment.NewLine + Environment.NewLine);
					}
				}
				throw new DocumenterException(sb.ToString());
			}
		}


		#region Global Xml Elements

		// writes out the default thead safety settings for the project
		private void WriteDefaultThreadSafety(XmlWriter writer)
		{
			writer.WriteStartElement("threadsafety");
			writer.WriteAttributeString("static", XmlConvert.ToString(this.rep.StaticMembersDefaultToSafe));
			writer.WriteAttributeString("instance", XmlConvert.ToString(this.rep.InstanceMembersDefaultToSafe));
			writer.WriteEndElement();
		}

		private void WriteFeedBackEmailAddress(XmlWriter writer)
		{
			writer.WriteElementString("feedbackEmail", this.rep.FeedbackEmailAddress);
		}
		
		// writes the copyright node to the documentation
		private void WriteCopyright(XmlWriter writer)
		{
			writer.WriteStartElement("copyright");
			writer.WriteAttributeString("text", this.rep.CopyrightText);

			if (this.rep.CopyrightHref.Length > 0)
			{
				if (!this.rep.CopyrightHref.StartsWith("http:"))
				{
					writer.WriteAttributeString("href", Path.GetFileName(this.rep.CopyrightHref));
				}
				else
				{
					writer.WriteAttributeString("href", this.rep.CopyrightHref);
				}
			}

			writer.WriteEndElement();
		}

		#endregion

		#region EditorBrowsable filter

		//checks if the member has been flagged with the 
		//EditorBrowsableState.Never value
		private bool IsEditorBrowsable(MemberInfo minfo)
		{
			if (this.rep.EditorBrowsableFilter == EditorBrowsableFilterLevel.Off)
			{
				return true;
			}

			EditorBrowsableAttribute[] browsables = 
				Attribute.GetCustomAttributes(minfo, typeof(EditorBrowsableAttribute), false)
				as EditorBrowsableAttribute[];
			
			if (browsables.Length == 0)
			{
				return true;
			}
			else
			{
				EditorBrowsableAttribute browsable = browsables[0];
				return (browsable.State == EditorBrowsableState.Always) || 
					((browsable.State == EditorBrowsableState.Advanced) && 
					(this.rep.EditorBrowsableFilter != EditorBrowsableFilterLevel.HideAdvanced));
			}
		}

		#endregion

		#region MustDocument * filters

		private bool MustDocumentType(Type type)
		{
			Type declaringType = type.DeclaringType;

			//If type name starts with a digit it is not a valid identifier
			//in any of the MS .Net languages.
			//It's probably a J# anonomous inner class...
			//Whatever, do not document it.
			if (Char.IsDigit(type.Name, 0))
				return false;

#if NET_2_0
			//If the type has a CompilerGenerated attribute then we don't want to document it 
			//as it is an internal artifact of the compiler
			if (type.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
			{
				return false;
			}
#else
			//HACK: exclude Net 2.0 compiler generated iterators
			//These are nested classes with name starting with "<"
			if (type.DeclaringType != null && type.Name.StartsWith("<"))
			{
				return false;
			}
#endif

			//exclude types that are internal to the .Net framework.
			if (type.FullName.StartsWith("System.") || type.FullName.StartsWith("Microsoft."))
			{
				if (type.IsNotPublic) return false;
				if (type.DeclaringType != null && 
					!MustDocumentType(type.DeclaringType))
					return false;
				// There are a group of *public* interfaces in System.Runtime.InteropServices
				// that are not documented by MS and should be considered internal to the framework...
				if (type.IsInterface && type.Namespace == "System.Runtime.InteropServices" && type.Name.StartsWith("_"))
					return false;
			}

			return 
				!type.FullName.StartsWith("<PrivateImplementationDetails>") && 
				(declaringType == null || MustDocumentType(declaringType)) && 
				(
				(type.IsPublic) || 
				(type.IsNotPublic && this.rep.DocumentInternals) || 
				(type.IsNestedPublic) || 
				(type.IsNestedFamily && this.rep.DocumentProtected) || 
				(type.IsNestedFamORAssem && this.rep.DocumentProtected) || 
				(type.IsNestedAssembly && this.rep.DocumentInternals) || 
				(type.IsNestedFamANDAssem && this.rep.DocumentInternals) || 
				(type.IsNestedPrivate && this.rep.DocumentPrivates)
				) && 
				IsEditorBrowsable(type) && 
				(!this.rep.UseNamespaceDocSummaries || (type.Name != "NamespaceDoc")) && 
				!assemblyDocCache.HasExcludeTag(MemberID.GetMemberID(type));
		}

		private bool MustDocumentMethod(MethodBase method)
		{
			//Ignore MC++ destructor.
			//The __dtor function is just a wrapper that just calls the 
			//Finalize method; all code you write in the destructor is 
			//actually written to the finalize method. So, we will filter
			//it out of the documentation by default...
			if (method.Name == "__dtor") 
				return false;
			
			//check the basic visibility options
			if (!
				(
				(method.IsPublic) || 
				(method.IsFamily && this.rep.DocumentProtected && 
				(this.rep.DocumentSealedProtected || !method.ReflectedType.IsSealed)) || 
				(method.IsFamilyOrAssembly && this.rep.DocumentProtected) || 
                (method.ReflectedType.Assembly == method.DeclaringType.Assembly &&
                    ((method.IsAssembly && this.rep.DocumentInternals) || 
				    (method.IsFamilyAndAssembly && this.rep.DocumentInternals))) || 
				(method.IsPrivate)
				)
				)
			{
				return false;
			}

			//HACK: exclude Net 2.0 Anonymous Methods
			//These have name starting with "<"
			if (method.Name.StartsWith("<"))
			{
				return false;
			}

			//Inherited Framework Members
			if ((!this.rep.DocumentInheritedFrameworkMembers) && 
				(method.ReflectedType != method.DeclaringType) && 
				(method.DeclaringType.FullName.StartsWith("System.") || 
				method.DeclaringType.FullName.StartsWith("Microsoft.")))
			{
				return false;
			}

			
			// Methods containing '.' in their name that aren't constructors are probably
			// explicit interface implementations, we check whether we document those or not.
			if ((method.Name.IndexOf('.') != -1) && 
				(method.Name != ".ctor") && 
				(method.Name != ".cctor") && 
				this.rep.DocumentExplicitInterfaceImplementations)
			{
				string interfaceName = null;
				int lastIndexOfDot = method.Name.LastIndexOf('.');
				if (lastIndexOfDot != -1)
				{
					interfaceName = method.Name.Substring(0, lastIndexOfDot);

					Type interfaceType = method.ReflectedType.GetInterface(interfaceName);

					// Document method if interface is (public) or (isInternal and documentInternal).
					if (interfaceType != null && (interfaceType.IsPublic || 
						(interfaceType.IsNotPublic && this.rep.DocumentInternals)))
					{
						return IsEditorBrowsable(method);
					}
				}
			}
			else
			{
				if (method.IsPrivate && !this.rep.DocumentPrivates)
					return false;
			}


			//check if the member has an exclude tag
			if (method.DeclaringType != method.ReflectedType) // inherited
			{
				if (assemblyDocCache.HasExcludeTag(GetMemberName(method, method.DeclaringType)))
					return false;
			}
			else
			{
				if (assemblyDocCache.HasExcludeTag(MemberID.GetMemberID(method)))
					return false;
			}

			return IsEditorBrowsable(method);
		}

		private bool MustDocumentProperty(PropertyInfo property)
		{
			// here we decide if the property is to be documented
			// note that we cannot directly test 'visibility' - it has to
			// be done for both the accessors individualy...
			if (IsEditorBrowsable(property))
			{
				MethodInfo getMethod = null;
				MethodInfo setMethod = null;
				if (property.CanRead)
				{
					try { getMethod = property.GetGetMethod(true); }
					catch (System.Security.SecurityException) {}
				}
				if (property.CanWrite)
				{
					try { setMethod = property.GetSetMethod(true); }
					catch (System.Security.SecurityException) {}
				}

				bool hasGetter = (getMethod != null) && MustDocumentMethod(getMethod);
				bool hasSetter = (setMethod != null) && MustDocumentMethod(setMethod);

				bool IsExcluded = false;
				//check if the member has an exclude tag
				if (property.DeclaringType != property.ReflectedType) // inherited
				{
					IsExcluded = assemblyDocCache.HasExcludeTag(GetMemberName(property, property.DeclaringType));
				}
				else
				{
					IsExcluded = assemblyDocCache.HasExcludeTag(MemberID.GetMemberID(property));
				}

				if ((hasGetter || hasSetter)
					&& !IsExcluded)
					return true;
			}
			return false;
		}


		private bool MustDocumentField(FieldInfo field)
		{
			if (!
				(
				(field.IsPublic) || 
				(field.IsFamily && this.rep.DocumentProtected && 
				(this.rep.DocumentSealedProtected || !field.ReflectedType.IsSealed)) || 
				(field.IsFamilyOrAssembly && this.rep.DocumentProtected) || 
                (field.ReflectedType.Assembly == field.DeclaringType.Assembly &&
				    ((field.IsAssembly && this.rep.DocumentInternals) || 
				    (field.IsFamilyAndAssembly && this.rep.DocumentInternals))) || 
				(field.IsPrivate && this.rep.DocumentPrivates))
				)
			{
				return false;
			}

			//HACK: exclude Net 2.0 Anonymous Method Delegates
			//These have name starting with "<"
			if (field.Name.StartsWith("<"))
			{
				return false;
			}

			if ((!this.rep.DocumentInheritedFrameworkMembers) && 
				(field.ReflectedType != field.DeclaringType) && 
				(field.DeclaringType.FullName.StartsWith("System.") || 
				field.DeclaringType.FullName.StartsWith("Microsoft.")))
			{
				return false;
			}

			//check if the member has an exclude tag
			if (field.DeclaringType != field.ReflectedType) // inherited
			{
				if (assemblyDocCache.HasExcludeTag(GetMemberName(field, field.DeclaringType)))
					return false;
			}
			else
			{
				if (assemblyDocCache.HasExcludeTag(MemberID.GetMemberID(field)))
					return false;
			}

			return IsEditorBrowsable(field);
		}

		#endregion

		#region IsHidden\IsHiding
		
		private bool IsHidden(MemberInfo member, Type type)
		{
			if (member.DeclaringType == member.ReflectedType)
				return false;

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic;

			MemberInfo[] members = type.GetMember(member.Name, bindingFlags);
			foreach (MemberInfo m in members)
			{
				if ((m != member)
					&& m.DeclaringType.IsSubclassOf(member.DeclaringType))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsHidden(MethodInfo method, Type type)
		{
			if (method.DeclaringType == method.ReflectedType)
				return false;

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic;

			MemberInfo[] members = type.GetMember(method.Name, bindingFlags);
			foreach (MemberInfo m in members)
			{
				if ((m != method)
					&& (m.DeclaringType.IsSubclassOf(method.DeclaringType))
					&& ((m.MemberType != MemberTypes.Method)
					|| HaveSameSig(m as MethodInfo, method)))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsHiding(MemberInfo member, Type type)
		{
			if (member.DeclaringType != member.ReflectedType)
				return false;

			Type baseType = type.BaseType;
			if (baseType == null)
				return false;

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic;

			MemberInfo[] members = baseType.GetMember(member.Name, bindingFlags);
			if (members.Length > 0)
				return true;

			return false;
		}

		private bool IsHiding(MethodInfo method, Type type)
		{
			if (method.DeclaringType != method.ReflectedType)
				return false;

			Type baseType = type.BaseType;
			if (baseType == null)
				return false;

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic;

			MemberInfo[] members = baseType.GetMember(method.Name, bindingFlags);
			foreach (MemberInfo m in members)
			{
				if (m == method)
					continue;

				if (m.MemberType != MemberTypes.Method)
					return true;

				MethodInfo meth = m as MethodInfo;
				if (HaveSameSig(meth, method)
					&& (((method.Attributes & MethodAttributes.Virtual) == 0)
					|| ((method.Attributes & MethodAttributes.NewSlot) != 0)))
				{
					return true;
				}
			}

			return false;
		}

		private bool IsHiding(PropertyInfo property, Type type)
		{
			if (!IsHiding((MemberInfo)property, type))
				return false;

			bool isIndexer = (property.Name == "Item");
			foreach (MethodInfo accessor in property.GetAccessors(true))
			{
				if (((accessor.Attributes & MethodAttributes.Virtual) != 0)
					&& ((accessor.Attributes & MethodAttributes.NewSlot) == 0))
					return false;

				// indexers only hide indexers with the same signature
				if (isIndexer && !IsHiding(accessor, type))
					return false;
			}

			return true;
		}

		private bool HaveSameSig(MethodInfo m1, MethodInfo m2)
		{
			ParameterInfo[] ps1 = m1.GetParameters();
			ParameterInfo[] ps2 = m2.GetParameters();

			if (ps1.Length != ps2.Length)
				return false;

			for (int i = 0; i < ps1.Length; i++)
			{
				ParameterInfo p1 = ps1[i];
				ParameterInfo p2 = ps2[i];
				if (p1.ParameterType != p2.ParameterType)
					return false;
				if (p1.IsIn != p2.IsIn)
					return false;
				if (p1.IsOut != p2.IsOut)
					return false;
				if (p1.IsRetval != p2.IsRetval)
					return false;
			}

			return true;
		}

		#endregion

		#region Assembly
		private void WriteAssembly(XmlWriter writer, Assembly assembly)
		{
			AssemblyName assemblyName = assembly.GetName();

			writer.WriteStartElement("assembly");
			writer.WriteAttributeString("name", assemblyName.Name);

			if (this.rep.AssemblyVersionInfo == AssemblyVersionInformationType.AssemblyVersion)
			{
				writer.WriteAttributeString("version", assemblyName.Version.ToString());
			}
			if (this.rep.AssemblyVersionInfo == AssemblyVersionInformationType.AssemblyFileVersion)
			{
				object[] attrs = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				if (attrs.Length > 0)
				{
					string version = ((AssemblyFileVersionAttribute)attrs[0]).Version;
					writer.WriteAttributeString("version", version);
				}
			}

			WriteCustomAttributes(writer, assembly);

			foreach (Module module in assembly.GetModules())
			{
				WriteModule(writer, module);
			}

			writer.WriteEndElement(); // assembly
		}
		#endregion

		#region Module
		/// <summary>Writes documentation about a module out as XML.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="module">Module to document.</param>
		private void WriteModule(XmlWriter writer, Module module)
		{
			writer.WriteStartElement("module");
			writer.WriteAttributeString("name", module.ScopeName);
			WriteCustomAttributes(writer, module);
			WriteNamespaces(writer, module);
			writer.WriteEndElement();
		}
		#endregion

		#region Namespace
		private void WriteNamespaces(XmlWriter writer, Module module)
		{
			Type[] types = module.GetTypes();

			StringCollection namespaceNames = GetNamespaceNames(types);

			foreach (string namespaceName in namespaceNames)
			{
				string ourNamespaceName;

				if (namespaceName == null)
				{
					ourNamespaceName = "(global)";
				}
				else
				{
					ourNamespaceName = namespaceName;
				}

				if (notEmptyNamespaces.ContainsKey(ourNamespaceName) || this.rep.DocumentEmptyNamespaces)
				{

					string namespaceSummary = null;
					if (this.rep.UseNamespaceDocSummaries)
					{
						if (namespaceName == null)
							namespaceSummary = assemblyDocCache.GetDoc("T:NamespaceDoc");
						else
							namespaceSummary = assemblyDocCache.GetDoc("T:" + namespaceName + ".NamespaceDoc");
					}

					bool isNamespaceDoc = false;

					if ((namespaceSummary == null) || (namespaceSummary.Length == 0))
						namespaceSummary = this.rep.NamespaceSummaries[ourNamespaceName] as string;
					else
						isNamespaceDoc = true;

					if (this.rep.SkipNamespacesWithoutSummaries && 
						(namespaceSummary == null || namespaceSummary.Length == 0))
					{
						Trace.WriteLine(string.Format("Skipping namespace {0} because it has no summary...", namespaceName));
					}
					else
					{
						Trace.WriteLine(string.Format("Writing namespace {0}...", namespaceName));

						writer.WriteStartElement("namespace");
						writer.WriteAttributeString("name", ourNamespaceName);

						if (namespaceSummary != null && namespaceSummary.Length > 0)
						{
							WriteStartDocumentation(writer);

							if (isNamespaceDoc)
							{
								writer.WriteRaw(namespaceSummary);
							}
							else
							{
								writer.WriteStartElement("summary");
								writer.WriteRaw(namespaceSummary);
								writer.WriteEndElement();
							}
							WriteEndDocumentation(writer);
						}
						else if (this.rep.ShowMissingSummaries)
						{
							WriteStartDocumentation(writer);
							WriteMissingDocumentation(writer, "summary", null, "Missing <summary> Documentation for " + namespaceName);
							WriteEndDocumentation(writer);
						}

						int classCount = WriteClasses(writer, types, namespaceName);
						Trace.WriteLine(string.Format("Wrote {0} classes.", classCount));

						int interfaceCount = WriteInterfaces(writer, types, namespaceName);
						Trace.WriteLine(string.Format("Wrote {0} interfaces.", interfaceCount));

						int structureCount = WriteStructures(writer, types, namespaceName);
						Trace.WriteLine(string.Format("Wrote {0} structures.", structureCount));

						int delegateCount = WriteDelegates(writer, types, namespaceName);
						Trace.WriteLine(string.Format("Wrote {0} delegates.", delegateCount));

						int enumCount = WriteEnumerations(writer, types, namespaceName);
						Trace.WriteLine(string.Format("Wrote {0} enumerations.", enumCount));

						writer.WriteEndElement();
					}
				}
				else
				{
					Trace.WriteLine(string.Format("Discarding namespace {0} because it does not contain any documented types.", ourNamespaceName));
				}
			}
		}
		#endregion

		#region TypeCollections
		private int WriteClasses(XmlWriter writer, Type[] types, string namespaceName)
		{
			int nbWritten = 0;

			foreach (Type type in types)
			{
				if (type.IsClass && 
					!IsDelegate(type) && 
					type.Namespace == namespaceName) 
				{
					string typeID = MemberID.GetMemberID(type);
					if (!documentedTypes.ContainsKey(typeID))
					{
						documentedTypes.Add(typeID, null);
						if (MustDocumentType(type))
						{
							bool hiding = ((type.MemberType & MemberTypes.NestedType) != 0)
								&& IsHiding(type, type.DeclaringType);
							WriteClass(writer, type, hiding);
							nbWritten++;
						}
					}
					else
					{
						Trace.WriteLine(typeID + " already documented - skipped...");
					}
				}
			}

			return nbWritten;
		}

		private int WriteInterfaces(XmlWriter writer, Type[] types, string namespaceName)
		{
			int nbWritten = 0;

			foreach (Type type in types)
			{
				if (type.IsInterface && 
					type.Namespace == namespaceName && 
					MustDocumentType(type))
				{
					string typeID = MemberID.GetMemberID(type);
					if (!documentedTypes.ContainsKey(typeID))
					{
						documentedTypes.Add(typeID, null);
						WriteInterface(writer, type);
						nbWritten++;
					}
					else
					{
						Trace.WriteLine(typeID + " already documented - skipped...");
					}
				}
			}

			return nbWritten;
		}

		private int WriteStructures(XmlWriter writer, Type[] types, string namespaceName)
		{
			int nbWritten = 0;

			foreach (Type type in types)
			{
				if (type.IsValueType && 
					!type.IsEnum && 
					type.Namespace == namespaceName && 
					MustDocumentType(type))
				{
					string typeID = MemberID.GetMemberID(type);
					if (!documentedTypes.ContainsKey(typeID))
					{
						documentedTypes.Add(typeID, null);
						bool hiding = ((type.MemberType & MemberTypes.NestedType) != 0)
							&& IsHiding(type, type.DeclaringType);
						WriteClass(writer, type, hiding);
						nbWritten++;
					}
					else
					{
						Trace.WriteLine(typeID + " already documented - skipped...");
					}
				}
			}

			return nbWritten;
		}

		private int WriteDelegates(XmlWriter writer, Type[] types, string namespaceName)
		{
			int nbWritten = 0;

			foreach (Type type in types)
			{
				if (type.IsClass && 
					IsDelegate(type) && 
					type.Namespace == namespaceName && 
					MustDocumentType(type))
				{
					string typeID = MemberID.GetMemberID(type);
					if (!documentedTypes.ContainsKey(typeID))
					{
						documentedTypes.Add(typeID, null);
						WriteDelegate(writer, type);
						nbWritten++;
					}
					else
					{
						Trace.WriteLine(typeID + " already documented - skipped...");
					}
				}
			}

			return nbWritten;
		}

		private int WriteEnumerations(XmlWriter writer, Type[] types, string namespaceName)
		{
			int nbWritten = 0;

			foreach (Type type in types)
			{
				if (type.IsEnum && 
					type.Namespace == namespaceName && 
					MustDocumentType(type))
				{
					string typeID = MemberID.GetMemberID(type);
					if (!documentedTypes.ContainsKey(typeID))
					{
						documentedTypes.Add(typeID, null);
						WriteEnumeration(writer, type);
						nbWritten++;
					}
					else
					{
						Trace.WriteLine(typeID + " already documented - skipped...");
					}
				}
			}

			return nbWritten;
		}

		private bool IsDelegate(Type type)
		{
			if (type.BaseType == null) return false;
			return type.BaseType.FullName == "System.Delegate" || 
				type.BaseType.FullName == "System.MulticastDelegate";
		}

		#endregion

		private int GetMethodOverload(MethodInfo method, Type type)
		{
			int count = 0;
			int overload = 0;

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic;

			MemberInfo[] methods = type.GetMember(method.Name, MemberTypes.Method, bindingFlags);
			foreach (MethodInfo m in methods)
			{
				if (!IsHidden(m, type) && MustDocumentMethod(m))
				{
					++count;
				}

				if (m == method)
				{
					overload = count;
				}
			}

			return (count > 1) ? overload : 0;
		}

		private int GetPropertyOverload(PropertyInfo property, PropertyInfo[] properties)
		{
			int count = 0;
			int overload = 0;

			foreach (PropertyInfo p in properties)
			{
				if ((p.Name == property.Name)
					/*&& !IsHidden(p, properties)*/)
				{
					++count;
				}

				if (p == property)
				{
					overload = count;
				}
			}

			return (count > 1) ? overload : 0;
		}


		#region Types
		/// <summary>Writes XML documenting a class.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="type">Class to document.</param>
		/// <param name="hiding">true if hiding base members</param>
		private void WriteClass(XmlWriter writer, Type type, bool hiding)
		{
			bool isStruct = type.IsValueType;

			string memberName = MemberID.GetMemberID(type);

			string fullNameWithoutNamespace = type.FullName.Replace('+', '.');

			if (type.Namespace != null)
			{
				fullNameWithoutNamespace = fullNameWithoutNamespace.Substring(type.Namespace.Length + 1);
			}

			writer.WriteStartElement(isStruct ? "structure" : "class");
			writer.WriteAttributeString("name", fullNameWithoutNamespace);
			writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(type));
			writer.WriteAttributeString("namespace", type.Namespace);
			writer.WriteAttributeString("id", memberName);
			writer.WriteAttributeString("access", GetTypeAccessValue(type));

			if (hiding)
			{
				writer.WriteAttributeString("hiding", "true");
			}

			// structs can't be abstract and always derive from System.ValueType
			// so don't bother including those attributes.
			if (!isStruct)
			{
				if (type.IsAbstract)
				{
					writer.WriteAttributeString("abstract", "true");
				}

				if (type.IsSealed)
				{
					writer.WriteAttributeString("sealed", "true");
				}

				if (type.BaseType != null && type.BaseType.FullName != "System.Object")
				{
					writer.WriteAttributeString("baseType", type.BaseType.Name);
				}
			}

			WriteTypeDocumentation(writer, memberName, type);
			WriteCustomAttributes(writer, type);
			if (type.BaseType != null)
				WriteBaseType(writer, type.BaseType);
			WriteDerivedTypes(writer, type);

			implementations = new ImplementsCollection();

			//build a collection of the base type's interfaces
			//to determine which have been inherited
			StringCollection baseInterfaces = new StringCollection();
			if (type.BaseType != null)
			{
				foreach (Type baseInterfaceType in type.BaseType.GetInterfaces())
				{
					baseInterfaces.Add(baseInterfaceType.FullName);
				}
			}
 
			foreach (Type interfaceType in type.GetInterfaces())
			{
				if (MustDocumentType(interfaceType))
				{
					writer.WriteStartElement("implements");
					writer.WriteAttributeString("type", interfaceType.FullName.Replace('+', '.'));
					writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(interfaceType));
					writer.WriteAttributeString("namespace", interfaceType.Namespace);
					if (baseInterfaces.Contains(interfaceType.FullName))
					{
						writer.WriteAttributeString("inherited", "true");
					}
					writer.WriteEndElement();
 
					InterfaceMapping interfaceMap = type.GetInterfaceMap(interfaceType);
					int numberOfMethods = interfaceMap.InterfaceMethods.Length;
					for (int i = 0; i < numberOfMethods; i++)
					{
						if (interfaceMap.TargetMethods[i] != null)
						{
							string implementation = interfaceMap.TargetMethods[i].ToString();
							ImplementsInfo implements = new ImplementsInfo();
							implements.InterfaceMethod = interfaceMap.InterfaceMethods[i];
							implements.InterfaceType = interfaceMap.InterfaceType;
							implements.TargetMethod = interfaceMap.TargetMethods[i];
							implements.TargetType = interfaceMap.TargetType;
							implementations[implementation] = implements;
						}
					}
				}
			}

			WriteConstructors(writer, type);
			WriteStaticConstructor(writer, type);
			WriteFields(writer, type);
			WriteProperties(writer, type);
			WriteMethods(writer, type);
			WriteOperators(writer, type);
			WriteEvents(writer, type);

			implementations = null;

			writer.WriteEndElement();
		}

		/// <summary>Writes XML documenting an interface.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="type">Interface to document.</param>
		private void WriteInterface(XmlWriter writer, Type type)
		{
			string memberName = MemberID.GetMemberID(type);

			string fullNameWithoutNamespace = type.FullName.Replace('+', '.');

			if (type.Namespace != null)
			{
				fullNameWithoutNamespace = fullNameWithoutNamespace.Substring(type.Namespace.Length + 1);
			}

			writer.WriteStartElement("interface");
			writer.WriteAttributeString("name", fullNameWithoutNamespace);
			writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(type));
			writer.WriteAttributeString("namespace", type.Namespace);
			writer.WriteAttributeString("id", memberName);
			writer.WriteAttributeString("access", GetTypeAccessValue(type));

			WriteTypeDocumentation(writer, memberName, type);
			WriteCustomAttributes(writer, type);

			WriteDerivedTypes(writer, type);
			
			foreach (Type interfaceType in type.GetInterfaces())
			{
				if (MustDocumentType(interfaceType))
				{
					writer.WriteStartElement("implements");
					writer.WriteAttributeString("type", interfaceType.FullName.Replace('+', '.'));
					writer.WriteEndElement();
				}
			}
			
			WriteInterfaceImplementingTypes(writer, type);

			WriteProperties(writer, type);
			WriteMethods(writer, type);
			WriteEvents(writer, type);

			writer.WriteEndElement();
		}

		/// <summary>Writes XML documenting a delegate.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="type">Delegate to document.</param>
		private void WriteDelegate(XmlWriter writer, Type type)
		{
			string memberName = MemberID.GetMemberID(type);

			writer.WriteStartElement("delegate");
			writer.WriteAttributeString("name", GetNestedTypeName(type));
			writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(type));
			writer.WriteAttributeString("namespace", type.Namespace);
			writer.WriteAttributeString("id", memberName);
			writer.WriteAttributeString("access", GetTypeAccessValue(type));

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic;

			MethodInfo[] methods = type.GetMethods(bindingFlags);
			foreach (MethodInfo method in methods)
			{
				if (method.Name == "Invoke")
				{
					Type t = method.ReturnType;
					writer.WriteAttributeString("returnType", MemberID.GetTypeName(t));
					writer.WriteAttributeString("valueType", t.IsValueType.ToString().ToLower());

					WriteDelegateDocumentation(writer, memberName, type, method);
					WriteCustomAttributes(writer, type);

					foreach (ParameterInfo parameter in method.GetParameters())
					{
						WriteParameter(writer, MemberID.GetMemberID(method), parameter);
					}
				}
			}

			writer.WriteEndElement();
		}

		private string GetNestedTypeName(Type type)
		{
			int indexOfPlus = type.FullName.IndexOf('+');
			if (indexOfPlus != -1)
			{
				int lastIndexOfDot = type.FullName.LastIndexOf('.');
				return type.FullName.Substring(lastIndexOfDot + 1).Replace('+', '.');
			}
			else
			{
				return type.Name;
			}
		}

		/// <summary>Writes XML documenting an enumeration.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="type">Enumeration to document.</param>
		private void WriteEnumeration(XmlWriter writer, Type type)
		{
			string memberName = MemberID.GetMemberID(type);

			writer.WriteStartElement("enumeration");
			writer.WriteAttributeString("name", GetNestedTypeName(type));
			writer.WriteAttributeString("id", memberName);
			writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(type));
			writer.WriteAttributeString("namespace", type.Namespace);
			writer.WriteAttributeString("access", GetTypeAccessValue(type));

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic | 
					  BindingFlags.DeclaredOnly;

			foreach (FieldInfo field in type.GetFields(bindingFlags))
			{
				// Enums are normally based on Int32, but this is not a CLR requirement.
				// In fact, they may be based on any integer type. The value__ field
				// defines the enum's base type, so we will treat this seperately...
				if (field.Name == "value__")
				{
					if (field.FieldType.FullName != "System.Int32")
					{
						writer.WriteAttributeString("baseType", field.FieldType.FullName);
					}
					break;
				}
			}
			if (type.IsDefined(typeof(System.FlagsAttribute), false))
			{
				writer.WriteAttributeString("flags", "true");
			}

			WriteEnumerationDocumentation(writer, memberName);
			WriteCustomAttributes(writer, type);

			foreach (FieldInfo field in type.GetFields(bindingFlags))
			{
				if (field.Name == "value__" || !IsEditorBrowsable (field))
					continue;

				WriteField(writer, field, type, IsHiding(field, type));
			}

			writer.WriteEndElement();
		}

		#endregion

		#region Attributes
		private void WriteStructLayoutAttribute(XmlWriter writer, Type type) 
		{
			string charSet = null;
			string layoutKind = null;

			if (!attributeFilter.Show("System.Runtime.InteropServices.StructLayoutAttribute", "CharSet"))
			{
				// determine if CharSet property should be documented
				if ((type.Attributes & TypeAttributes.AutoClass) == TypeAttributes.AutoClass)
				{
					charSet = CharSet.Auto.ToString();
				}
				//			//Do not document if default value....
				//			if ((type.Attributes & TypeAttributes.AnsiClass) == TypeAttributes.AnsiClass)
				//			{
				//				charSet = CharSet.Ansi.ToString(CultureInfo.InvariantCulture);
				//			} 
				if ((type.Attributes & TypeAttributes.UnicodeClass) == TypeAttributes.UnicodeClass)
				{
					charSet = CharSet.Unicode.ToString();
				}
			}

			if (!attributeFilter.Show("System.Runtime.InteropServices.StructLayoutAttribute", "Value"))
			{
				// determine if Value property should be documented
				//			//Do not document if default value....
				//			if ((type.Attributes & TypeAttributes.AutoLayout) == TypeAttributes.AutoLayout)
				//			{
				//				layoutKind = LayoutKind.Auto.ToString(CultureInfo.InvariantCulture);
				//			} 
				if ((type.Attributes & TypeAttributes.ExplicitLayout) == TypeAttributes.ExplicitLayout)
				{
					layoutKind = LayoutKind.Explicit.ToString();
				} 
				if ((type.Attributes & TypeAttributes.SequentialLayout) == TypeAttributes.SequentialLayout)
				{
					layoutKind = LayoutKind.Sequential.ToString();
				}
			}
			
			if (charSet == null && layoutKind == null)
			{
				return;
			}

			// create attribute element
			writer.WriteStartElement("attribute");
			writer.WriteAttributeString("name", "System.Runtime.InteropServices.StructLayoutAttribute");

			if (charSet != null)
			{
				// create CharSet property element
				writer.WriteStartElement("property");
				writer.WriteAttributeString("name", "CharSet");
				writer.WriteAttributeString("type", "System.Runtime.InteropServices.CharSet");
				writer.WriteAttributeString("value", charSet);
				writer.WriteEndElement();
			}

			if (layoutKind != null) 
			{
				// create Value property element
				writer.WriteStartElement("property");
				writer.WriteAttributeString("name", "Value");
				writer.WriteAttributeString("type", "System.Runtime.InteropServices.LayoutKind");
				writer.WriteAttributeString("value", layoutKind);
				writer.WriteEndElement();
			}

			// end attribute element
			writer.WriteEndElement();

		}

		private void WriteSpecialAttributes(XmlWriter writer, Type type)
		{
			if ((type.Attributes & TypeAttributes.Serializable) == TypeAttributes.Serializable)
			{
				if (attributeFilter.Show("System.SerializableAttribute"))
				{
					writer.WriteStartElement("attribute");
					writer.WriteAttributeString("name", "System.SerializableAttribute");
					writer.WriteEndElement(); // attribute
				}
			}

			WriteStructLayoutAttribute(writer, type);
		}

		private void WriteSpecialAttributes(XmlWriter writer, FieldInfo field)
		{
			if ((field.Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized)
			{
				if (attributeFilter.Show("System.NonSerializedAttribute"))
				{
					writer.WriteStartElement("attribute");
					writer.WriteAttributeString("name", "System.NonSerializedAttribute");
					writer.WriteEndElement(); // attribute
				}
			}

			//TODO: more special attributes here?
		}

		private void WriteCustomAttributes(XmlWriter writer, Assembly assembly) 
		{
			WriteCustomAttributes(writer, assembly.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "assembly");
		}

		private void WriteCustomAttributes(XmlWriter writer, Module module) 
		{
			WriteCustomAttributes(writer, module.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "module");
		}

		private void WriteCustomAttributes(XmlWriter writer, Type type)
		{
			try
			{
				WriteSpecialAttributes(writer, type);
				WriteCustomAttributes(writer, type.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "");
			}
			catch (Exception e)
			{
				TraceErrorOutput("Error retrieving custom attributes for " + MemberID.GetMemberID(type), e);
			}
		}

		private void WriteCustomAttributes(XmlWriter writer, FieldInfo fieldInfo)
		{
			try
			{
				WriteSpecialAttributes(writer, fieldInfo);
				WriteCustomAttributes(writer, fieldInfo.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "");
			}
			catch (Exception e)
			{
				TraceErrorOutput("Error retrieving custom attributes for " + MemberID.GetMemberID(fieldInfo), e);
			}
		}

		private void WriteCustomAttributes(XmlWriter writer, ConstructorInfo constructorInfo)
		{
			try
			{
				WriteCustomAttributes(writer, constructorInfo.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "");
			}
			catch (Exception e)
			{
				TraceErrorOutput("Error retrieving custom attributes for " + MemberID.GetMemberID(constructorInfo), e);
			}
		}

		private void WriteCustomAttributes(XmlWriter writer, MethodInfo methodInfo)
		{
			try
			{
				WriteCustomAttributes(writer, methodInfo.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "");
				WriteCustomAttributes(writer, methodInfo.ReturnTypeCustomAttributes.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "return");
			}
			catch (Exception e)
			{
				TraceErrorOutput("Error retrieving custom attributes for " + MemberID.GetMemberID(methodInfo), e);
			}
		}

		private void WriteCustomAttributes(XmlWriter writer, PropertyInfo propertyInfo)
		{
			try
			{
				WriteCustomAttributes(writer, propertyInfo.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "");
			}
			catch (Exception e)
			{
				TraceErrorOutput("Error retrieving custom attributes for " + MemberID.GetMemberID(propertyInfo), e);
			}
		}

		private void WriteCustomAttributes(XmlWriter writer, ParameterInfo parameterInfo)
		{
			try
			{
				WriteCustomAttributes(writer, parameterInfo.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "");
			}
			catch (Exception e)
			{
				TraceErrorOutput("Error retrieving custom attributes for " + parameterInfo.Member.ReflectedType.FullName + "." + parameterInfo.Member.Name + " param " + parameterInfo.Name, e);
			}
		}

		private void WriteCustomAttributes(XmlWriter writer, EventInfo eventInfo)
		{
			try
			{
				WriteCustomAttributes(writer, eventInfo.GetCustomAttributes(this.rep.DocumentInheritedAttributes), "");
			}
			catch (Exception e)
			{
				TraceErrorOutput("Error retrieving custom attributes for " + MemberID.GetMemberID(eventInfo), e);
			}
		}

		private void WriteCustomAttributes(XmlWriter writer, object[] attributes, string target)
		{
			foreach (Attribute attribute in attributes)
			{
				if (this.rep.DocumentAttributes)
				{
					if (MustDocumentType(attribute.GetType()) && attributeFilter.Show(attribute.GetType().FullName))
					{
						WriteCustomAttribute(writer, attribute, target);
					}
				}

				if (attribute.GetType().FullName == "System.ObsoleteAttribute") 
				{
					writer.WriteElementString("obsolete", ((ObsoleteAttribute)attribute).Message);
				}
			}
		}

		private void WriteCustomAttribute(XmlWriter writer, Attribute attribute, string target)
		{
			writer.WriteStartElement("attribute");
			string fullName = attribute.GetType().FullName;
			writer.WriteAttributeString("name", fullName);
			if (target.Length > 0)
			{
				writer.WriteAttributeString("target", target);
			}

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Public;

			foreach (FieldInfo field in attribute.GetType().GetFields(bindingFlags))
			{
				if (MustDocumentField(field) && attributeFilter.Show(fullName, field.Name))
				{
					string fieldValue = null;
					try
					{
						fieldValue = GetDisplayValue(field.DeclaringType, field.GetValue(attribute));
					}
					catch (Exception e)
					{
						TraceErrorOutput("Value for attribute field " + MemberID.GetMemberID(field).Substring(2) + " cannot be determined", e);
						fieldValue = "***UNKNOWN***";
					}
					if (fieldValue.Length > 0)
					{
						writer.WriteStartElement("field");
						writer.WriteAttributeString("name", field.Name);
						writer.WriteAttributeString("type", field.FieldType.FullName);
						writer.WriteAttributeString("value", fieldValue);
						writer.WriteEndElement(); // field
					}
				}
			}

			foreach (PropertyInfo property in attribute.GetType().GetProperties(bindingFlags))
			{
				//skip the TypeId property
				if ((!this.rep.ShowTypeIdInAttributes) && (property.Name == "TypeId"))
				{
					continue;
				}

				if (MustDocumentProperty(property) && attributeFilter.Show(fullName, property.Name))
				{
					if (property.CanRead)
					{
						string propertyValue = null;
						try
						{
							propertyValue = GetDisplayValue(property.DeclaringType, property.GetValue(attribute, null));
						}
						catch (Exception e)
						{
							TraceErrorOutput("Value for attribute property " + MemberID.GetMemberID(property).Substring(2) + " cannot be determined", e);
							propertyValue = "***UNKNOWN***";
						}
						if (propertyValue.Length > 0)
						{
							writer.WriteStartElement("property");
							writer.WriteAttributeString("name", property.Name);
							writer.WriteAttributeString("type", property.PropertyType.FullName);
							writer.WriteAttributeString("value", propertyValue);
							writer.WriteEndElement(); // property
						}
					}
				}
			}

			writer.WriteEndElement(); // attribute
		}

		#endregion

		#region MemberCollections

		private void WriteConstructors(XmlWriter writer, Type type)
		{
			int overload = 0;

			BindingFlags bindingFlags = 
				BindingFlags.Instance | 
				BindingFlags.Public | 
				BindingFlags.NonPublic;

			if (!this.rep.DocumentInheritedMembers)
			{
				bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
			}

			ConstructorInfo[] constructors = type.GetConstructors(bindingFlags);

			if (constructors.Length > 1)
			{
				overload = 1;
			}

			foreach (ConstructorInfo constructor in constructors)
			{
				if (MustDocumentMethod(constructor))
				{
					WriteConstructor(writer, constructor, overload++);
				}
			}
		}

		private void WriteStaticConstructor(XmlWriter writer, Type type)
		{
			BindingFlags bindingFlags = 
				BindingFlags.Static | 
				BindingFlags.Public | 
				BindingFlags.NonPublic;

			if (!this.rep.DocumentInheritedMembers)
			{
				bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
			}

			ConstructorInfo[] constructors = type.GetConstructors(bindingFlags);

			foreach (ConstructorInfo constructor in constructors)
			{
				if (MustDocumentMethod(constructor))
				{
					WriteConstructor(writer, constructor, 0);
				}
			}
		}

		private void WriteFields(XmlWriter writer, Type type)
		{
			BindingFlags bindingFlags = 
				BindingFlags.Instance | 
				BindingFlags.Static | 
				BindingFlags.Public | 
				BindingFlags.NonPublic;

			if (!this.rep.DocumentInheritedMembers)
			{
				bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
			}

			FieldInfo[] fields = type.GetFields(bindingFlags);
			foreach (FieldInfo field in fields)
			{
				if (MustDocumentField(field)
					&& !IsAlsoAnEvent(field)
					&& !IsHidden(field, type))
				{
					WriteField(
						writer, 
						field, 
						type, 
						IsHiding(field, type));
				}
			}
		}

		private void WriteProperties(XmlWriter writer, Type type)
		{
			BindingFlags bindingFlags = 
				BindingFlags.Instance | 
				BindingFlags.Static | 
				BindingFlags.Public | 
				BindingFlags.NonPublic;

			if (!this.rep.DocumentInheritedMembers)
			{
				bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
			}

			PropertyInfo[] properties = type.GetProperties(bindingFlags);

			foreach (PropertyInfo property in properties)
			{
				if (MustDocumentProperty(property)
					&& !IsAlsoAnEvent(property)
					&& !IsHidden(property, type)
					)
				{
					WriteProperty(
						writer, 
						property, 
						property.DeclaringType.FullName != type.FullName, 
						GetPropertyOverload(property, properties), 
						IsHiding(property, type));
				}
				
			}
		}

		private void WriteMethods(XmlWriter writer, Type type)
		{
			BindingFlags bindingFlags = 
				BindingFlags.Instance | 
				BindingFlags.Static | 
				BindingFlags.Public | 
				BindingFlags.NonPublic;

			if (!this.rep.DocumentInheritedMembers)
			{
				bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
			}

			MethodInfo[] methods = type.GetMethods(bindingFlags);

			foreach (MethodInfo method in methods)
			{
				string name = method.Name;

				int lastIndexOfDot = name.LastIndexOf('.');

				if (lastIndexOfDot != -1)
				{
					name = method.Name.Substring(lastIndexOfDot + 1);
				}

				if (
					!(
					method.IsSpecialName && 
					(
					name.StartsWith("get_") || 
					name.StartsWith("set_") || 
					name.StartsWith("add_") || 
					name.StartsWith("remove_") || 
					name.StartsWith("raise_") || 
					name.StartsWith("op_")
					)
					) && 
					MustDocumentMethod(method) && 
					!IsHidden(method, type))
				{
					WriteMethod(
						writer, 
						method, 
						method.DeclaringType.FullName != type.FullName, 
						GetMethodOverload(method, type), 
						IsHiding(method, type));
				}
			}
		}

		private void WriteOperators(XmlWriter writer, Type type)
		{
			BindingFlags bindingFlags = 
				BindingFlags.Instance | 
				BindingFlags.Static | 
				BindingFlags.Public | 
				BindingFlags.NonPublic;

			if (!this.rep.DocumentInheritedMembers)
			{
				bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
			}

			MethodInfo[] methods = type.GetMethods(bindingFlags);
			foreach (MethodInfo method in methods)
			{
				if (method.Name.StartsWith("op_") && 
					MustDocumentMethod(method))
				{
					WriteOperator(
						writer, 
						method, 
						GetMethodOverload(method, type));
				}
			}
		}

		private void WriteEvents(XmlWriter writer, Type type)
		{
			BindingFlags bindingFlags = 
				BindingFlags.Instance | 
				BindingFlags.Static | 
				BindingFlags.Public | 
				BindingFlags.NonPublic;

			if (!this.rep.DocumentInheritedMembers)
			{
				bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
			}

			EventInfo[] events = type.GetEvents(bindingFlags);
			foreach (EventInfo eventInfo in events)
			{
				bool IsExcluded = false;
				//check if the event has an exclude tag
				if (eventInfo.DeclaringType != eventInfo.ReflectedType) // inherited
				{
					IsExcluded = assemblyDocCache.HasExcludeTag(GetMemberName(eventInfo, eventInfo.DeclaringType));
				}
				else
				{
					IsExcluded = assemblyDocCache.HasExcludeTag(MemberID.GetMemberID(eventInfo));
				}

				if (!IsExcluded)
				{
					MethodInfo addMethod = eventInfo.GetAddMethod(true);

					if (addMethod != null && 
						MustDocumentMethod(addMethod) && 
						IsEditorBrowsable(eventInfo))
					{
						WriteEvent(writer, eventInfo);
					}
				}
			}
		}

		
		private bool IsAlsoAnEvent(Type type, string fullName)
		{
			bool isEvent = false;

			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic | 
					  BindingFlags.DeclaredOnly;

			EventInfo[] events = type.GetEvents(bindingFlags);
			foreach (EventInfo eventInfo in events)
			{
				if (eventInfo.EventHandlerType.FullName == fullName)
				{
					isEvent = true;
					break;
				}
			}

			return isEvent;
		}

		private bool IsAlsoAnEvent(FieldInfo field)
		{
			return IsAlsoAnEvent(field.DeclaringType, field.FieldType.FullName);
		}

		private bool IsAlsoAnEvent(PropertyInfo property)
		{
			return IsAlsoAnEvent(property.DeclaringType, property.PropertyType.FullName);
		}

		private void WriteBaseType(XmlWriter writer, Type type)
		{
			if (!"System.Object".Equals(type.FullName))
			{
				writer.WriteStartElement("base");
				writer.WriteAttributeString("name", type.Name);
				writer.WriteAttributeString("id", MemberID.GetMemberID(type));
				writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(type));
				writer.WriteAttributeString("namespace", type.Namespace);

				WriteBaseType(writer, type.BaseType);

				writer.WriteEndElement();
			}
		}

		#endregion


		#region Members

		/// <summary>Writes XML documenting a field.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="field">Field to document.</param>
		/// <param name="type">Type containing the field.</param>
		/// <param name="hiding">true if hiding base members</param>
		private void WriteField(XmlWriter writer, FieldInfo field, Type type, bool hiding)
		{
			string memberName = MemberID.GetMemberID(field);

			writer.WriteStartElement("field");
			writer.WriteAttributeString("name", field.Name);
			writer.WriteAttributeString("id", memberName);
			writer.WriteAttributeString("access", GetFieldAccessValue(field));
			
			if (field.IsStatic)
			{
				writer.WriteAttributeString("contract", "Static");
			}
			else
			{
				writer.WriteAttributeString("contract", "Normal");
			}

			Type t = field.FieldType;
#if NET_2_0
            writer.WriteAttributeString("type", MemberID.GetTypeName(t, false));
#else
			writer.WriteAttributeString("type", MemberID.GetTypeName(t));
#endif
			writer.WriteAttributeString("valueType", t.IsValueType.ToString().ToLower());

			bool inherited = (field.DeclaringType != field.ReflectedType);
			if (inherited)
			{
				writer.WriteAttributeString("declaringType", MemberID.GetDeclaringTypeName(field));
			}

			if (!IsMemberSafe(field))
				writer.WriteAttributeString("unsafe", "true");

			if (hiding)
			{
				writer.WriteAttributeString("hiding", "true");
			}

			if (field.IsInitOnly)
			{
				writer.WriteAttributeString("initOnly", "true");
			}

			if (field.IsLiteral)
			{
				writer.WriteAttributeString("literal", "true");
				string fieldValue = null;
				try
				{
					fieldValue = GetDisplayValue(field.DeclaringType, field.GetValue(null));
				}
				catch (Exception e)
				{
					TraceErrorOutput("Literal value for " + memberName.Substring(2) + " cannot be determined", e);
				}
				if (fieldValue != null)
				{
					writer.WriteAttributeString("value", fieldValue);
				}
			}

			if (inherited)
			{
				WriteInheritedDocumentation(writer, memberName, field.DeclaringType);
			}
			else
			{
				WriteFieldDocumentation(writer, memberName, type);
			}
			WriteCustomAttributes(writer, field);

			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		protected string GetDisplayValue(Type parent, object value)
		{
			if (value == null) return "null";

			if (value is string)
			{
				return (value.ToString());
			}

			if (value is Enum)
			{
				if (parent.IsEnum)
				{
					return Enum.Format(value.GetType(), value, "d");
				}
				else
				{
					string enumTypeName = value.GetType().Name;
					string enumValue = value.ToString();
					string[] enumValues = enumValue.Split(new char[] {','});
					if (enumValues.Length > 1)
					{
						for (int i = 0; i < enumValues.Length; i++)
						{
							enumValues[i] = enumTypeName + "." + enumValues[i].Trim();
						}
						return "(" + String.Join("|", enumValues) + ")";
					}
					else
					{
						return enumTypeName + "." + enumValue;
					}
				}
			}

			return value.ToString();

		}

		/// <summary>Writes XML documenting an event.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="eventInfo">Event to document.</param>
		private void WriteEvent(XmlWriter writer, EventInfo eventInfo)
		{
			string memberName = MemberID.GetMemberID(eventInfo);

			string name = eventInfo.Name;
			string interfaceName = null;

			int lastIndexOfDot = name.LastIndexOf('.');
			if (lastIndexOfDot != -1)
			{
				//this is an explicit interface implementation. if we don't want
				//to document them, get out of here quick...
				if (!this.rep.DocumentExplicitInterfaceImplementations) return;

				interfaceName = name.Substring(0, lastIndexOfDot);
				lastIndexOfDot = interfaceName.LastIndexOf('.');
				if (lastIndexOfDot != -1)
					name = name.Substring(lastIndexOfDot + 1);

				//check if we want to document this interface.
				ImplementsInfo implements = null;
				MethodInfo adder = eventInfo.GetAddMethod(true);
				if (adder != null)
				{
					implements = implementations[adder.ToString()];
				}
				if (implements == null)
				{
					MethodInfo remover = eventInfo.GetRemoveMethod(true);
					if (remover != null)
					{
						implements = implementations[remover.ToString()];
					}
				}
				if (implements != null) return;
			}

			writer.WriteStartElement("event");
			writer.WriteAttributeString("name", name);
			writer.WriteAttributeString("id", memberName);
			writer.WriteAttributeString("access", GetMethodAccessValue(eventInfo.GetAddMethod(true)));
			writer.WriteAttributeString("contract", GetMethodContractValue(eventInfo.GetAddMethod(true)));
			Type t = eventInfo.EventHandlerType;
			writer.WriteAttributeString("type", MemberID.GetTypeName(t));
			writer.WriteAttributeString("valueType", t.IsValueType.ToString().ToLower());

			bool inherited = eventInfo.DeclaringType != eventInfo.ReflectedType;

			if (inherited)
			{
				writer.WriteAttributeString("declaringType", MemberID.GetDeclaringTypeName(eventInfo));
			}

			if (interfaceName != null)
			{
				writer.WriteAttributeString("interface", interfaceName);
			}

			if (eventInfo.IsMulticast)
			{
				writer.WriteAttributeString("multicast", "true");
			}

			if (inherited)
			{
				WriteInheritedDocumentation(writer, memberName, eventInfo.DeclaringType);
			}
			else
			{
				WriteEventDocumentation(writer, memberName, true);
			}
			WriteCustomAttributes(writer, eventInfo);

			if (implementations != null)
			{
				ImplementsInfo implements = null;
				MethodInfo adder = eventInfo.GetAddMethod(true);
				if (adder != null)
				{
					implements = implementations[adder.ToString()];
				}
				if (implements == null)
				{
					MethodInfo remover = eventInfo.GetRemoveMethod(true);
					if (remover != null)
					{
						implements = implementations[remover.ToString()];
					}
				}
				if (implements != null)
				{
					writer.WriteStartElement("implements");
					MemberInfo InterfaceMethod = (MemberInfo)implements.InterfaceMethod;
					EventInfo InterfaceEvent = 
						InterfaceMethod.DeclaringType.GetEvent(InterfaceMethod.Name.Substring(4));
					writer.WriteAttributeString("name", InterfaceEvent.Name);
					writer.WriteAttributeString("id", MemberID.GetMemberID(InterfaceEvent));
					writer.WriteAttributeString("interface", implements.InterfaceType.Name);
					writer.WriteAttributeString("interfaceId", MemberID.GetMemberID(implements.InterfaceType));
					writer.WriteAttributeString("declaringType", implements.InterfaceType.FullName.Replace('+', '.'));
					writer.WriteEndElement();
				}
			}

			writer.WriteEndElement();
		}

		/// <summary>Writes XML documenting a constructor.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="constructor">Constructor to document.</param>
		/// <param name="overload">If &gt; 0, indicates this is the nth overloaded constructor.</param>
		private void WriteConstructor(XmlWriter writer, ConstructorInfo constructor, int overload)
		{
			string memberName = MemberID.GetMemberID(constructor);

			writer.WriteStartElement("constructor");
			writer.WriteAttributeString("name", constructor.Name);
			writer.WriteAttributeString("id", memberName);
			writer.WriteAttributeString("access", GetMethodAccessValue(constructor));
			writer.WriteAttributeString("contract", GetMethodContractValue(constructor));
			
			if (overload > 0)
			{
				writer.WriteAttributeString("overload", overload.ToString());
			}

			if (!IsMemberSafe(constructor))
				writer.WriteAttributeString("unsafe", "true");

			WriteConstructorDocumentation(writer, memberName, constructor);
			WriteCustomAttributes(writer, constructor);

			foreach (ParameterInfo parameter in constructor.GetParameters())
			{
				WriteParameter(writer, MemberID.GetMemberID(constructor), parameter);
			}

			writer.WriteEndElement();
		}

		/// <summary>Writes XML documenting a property.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="property">Property to document.</param>
		/// <param name="inherited">true if a declaringType attribute should be included.</param>
		/// <param name="overload">If &gt; 0, indicates this it the nth overloaded method with the same name.</param>
		/// <param name="hiding">true if this property is hiding base class members with the same name.</param>
		private void WriteProperty(XmlWriter writer, PropertyInfo property, bool inherited, int overload, bool hiding)
		{
			if (property != null)
			{
				string memberName = MemberID.GetMemberID(property);

				string name = property.Name;
				string interfaceName = null;

				MethodInfo getter = property.GetGetMethod(true);
				MethodInfo setter = property.GetSetMethod(true);

				int lastIndexOfDot = name.LastIndexOf('.');
				if (lastIndexOfDot != -1)
				{
					//this is an explicit interface implementation. if we don't want
					//to document them, get out of here quick...
					if (!this.rep.DocumentExplicitInterfaceImplementations) return;

					interfaceName = name.Substring(0, lastIndexOfDot);
					lastIndexOfDot = interfaceName.LastIndexOf('.');
					if (lastIndexOfDot != -1)
						name = name.Substring(lastIndexOfDot + 1);

					//check if we want to document this interface.
					ImplementsInfo implements = null;
					if (getter != null)
					{
						implements = implementations[getter.ToString()];
					}
					if (implements == null)
					{
						if (setter != null)
						{
							implements = implementations[setter.ToString()];
						}
					}
					if (implements == null) return;
				}

				writer.WriteStartElement("property");
				writer.WriteAttributeString("name", name);
				writer.WriteAttributeString("id", memberName);
				writer.WriteAttributeString("access", GetPropertyAccessValue(property));
				writer.WriteAttributeString("contract", GetPropertyContractValue(property));
				Type t = property.PropertyType;
#if NET_2_0
                writer.WriteAttributeString("type", MemberID.GetTypeName(t, false));
#else
				writer.WriteAttributeString("type", MemberID.GetTypeName(t));
#endif
				writer.WriteAttributeString("valueType", t.IsValueType.ToString().ToLower());

				if (inherited)
				{
					writer.WriteAttributeString("declaringType", MemberID.GetDeclaringTypeName(property));
				}

				if (overload > 0)
				{
					writer.WriteAttributeString("overload", overload.ToString());
				}

				if (!IsMemberSafe(property))
					writer.WriteAttributeString("unsafe", "true");

				if (hiding)
				{
					writer.WriteAttributeString("hiding", "true");
				}

				if (interfaceName != null)
				{
					writer.WriteAttributeString("interface", interfaceName);
				}

				writer.WriteAttributeString("get", getter != null ? "true" : "false");
				writer.WriteAttributeString("set", setter != null ? "true" : "false");

				if (inherited)
				{
					WriteInheritedDocumentation(writer, memberName, property.DeclaringType);
				}
				else
				{
					WritePropertyDocumentation(writer, memberName, property, true);
				}
				WriteCustomAttributes(writer, property);
				if (getter != null)
				{
					WriteCustomAttributes(writer, getter.ReturnTypeCustomAttributes.GetCustomAttributes(true), "return");
				}

				foreach (ParameterInfo parameter in GetIndexParameters(property))
				{
					WriteParameter(writer, memberName, parameter);
				}

				if (implementations != null)
				{
					ImplementsInfo implements = null;
					if (getter != null)
					{
						implements = implementations[getter.ToString()];
					}
					if (implements == null)
					{
						if (setter != null)
						{
							implements = implementations[setter.ToString()];
						}
					}
					if (implements != null)
					{
						MethodInfo InterfaceMethod = (MethodInfo)implements.InterfaceMethod;
						PropertyInfo InterfaceProperty = DerivePropertyFromAccessorMethod(InterfaceMethod);
						if (InterfaceProperty != null)
						{
							string InterfacePropertyID = MemberID.GetMemberID(InterfaceProperty);
							writer.WriteStartElement("implements");
							writer.WriteAttributeString("name", InterfaceProperty.Name);
							writer.WriteAttributeString("id", InterfacePropertyID);
							writer.WriteAttributeString("interface", implements.InterfaceType.Name);
							writer.WriteAttributeString("interfaceId", MemberID.GetMemberID(implements.InterfaceType));
							writer.WriteAttributeString("declaringType", implements.InterfaceType.FullName.Replace('+', '.'));
							writer.WriteEndElement();
						}
						else if (InterfaceMethod != null)
						{
							string InterfaceMethodID = MemberID.GetMemberID(InterfaceMethod);
							writer.WriteStartElement("implements");
							writer.WriteAttributeString("name", InterfaceMethod.Name);
							writer.WriteAttributeString("id", InterfaceMethodID);
							writer.WriteAttributeString("interface", implements.InterfaceType.Name);
							writer.WriteAttributeString("interfaceId", MemberID.GetMemberID(implements.InterfaceType));
							writer.WriteAttributeString("declaringType", implements.InterfaceType.FullName.Replace('+', '.'));
							writer.WriteEndElement();
						}
					}
				}

				writer.WriteEndElement();
			}
		}

		private PropertyInfo DerivePropertyFromAccessorMethod(MemberInfo accessor)
		{
			MethodInfo accessorMethod = (MethodInfo)accessor;
			string accessortype = accessorMethod.Name.Substring(0, 3);
			string propertyName = accessorMethod.Name.Substring(4);

			ParameterInfo[] parameters;
			parameters = accessorMethod.GetParameters();
			int parmCount = parameters.GetLength(0);
			
			Type returnType = null;
			Type[] types = null;

			if (accessortype == "get")
			{
				returnType = accessorMethod.ReturnType;
				types = new Type[parmCount];
				for (int i = 0; i < parmCount; i++)
				{
					types[i] = ((ParameterInfo)parameters.GetValue(i)).ParameterType;
				}
			}
			else
			{
				returnType = ((ParameterInfo)parameters.GetValue(parmCount - 1)).ParameterType;
				parmCount--;
				types = new Type[parmCount];
				for (int i = 0; i < parmCount; i++)
				{
					types[i] = ((ParameterInfo)parameters.GetValue(i + 1)).ParameterType;
				}
			}

			PropertyInfo derivedProperty = accessorMethod.DeclaringType.GetProperty(propertyName, returnType, types);
			return derivedProperty;
		}

		private string GetPropertyContractValue(PropertyInfo property)
		{
			return GetMethodContractValue(property.GetAccessors(true)[0]);
		}

		private ParameterInfo[] GetIndexParameters(PropertyInfo property)
		{
			// The ParameterInfo[] returned by GetIndexParameters()
			// contains ParameterInfo objects with empty names so
			// we have to get the parameters from the getter or
			// setter instead.

			ParameterInfo[] parameters;
			int length = 0;

			if (property.GetGetMethod(true) != null)
			{
				parameters = property.GetGetMethod(true).GetParameters();

				if (parameters != null)
				{
					length = parameters.Length;
				}
			}
			else
			{
				parameters = property.GetSetMethod(true).GetParameters();

				if (parameters != null)
				{
					// If the indexer only has a setter, we neet
					// to subtract 1 so that the value parameter
					// isn't displayed.

					length = parameters.Length - 1;
				}
			}

			ParameterInfo[] result = new ParameterInfo[length];

			if (length > 0)
			{
				for (int i = 0; i < length; ++i)
				{
					result[i] = parameters[i];
				}
			}

			return result;
		}

		/// <summary>Writes XML documenting a method.</summary>
		/// <param name="writer">XmlWriter to write on.</param>
		/// <param name="method">Method to document.</param>
		/// <param name="inherited">true if a declaringType attribute should be included.</param>
		/// <param name="overload">If &gt; 0, indicates this it the nth overloaded method with the same name.</param>
		/// <param name="hiding">true if this method hides methods of the base class with the same signature.</param>
		private void WriteMethod(XmlWriter writer, MethodInfo method, bool inherited, int overload, bool hiding)
		{
			if (method != null)
			{
				string memberName = MemberID.GetMemberID(method);

				string name = method.Name;
				string interfaceName = null;

				name = name.Replace('+', '.');
				int lastIndexOfDot = name.LastIndexOf('.');
				if (lastIndexOfDot != -1)
				{
					//this is an explicit interface implementation. if we don't want
					//to document them, get out of here quick...
					if (!this.rep.DocumentExplicitInterfaceImplementations) return;

					interfaceName = name.Substring(0, lastIndexOfDot);
					lastIndexOfDot = interfaceName.LastIndexOf('.');
					if (lastIndexOfDot != -1)
						name = name.Substring(lastIndexOfDot + 1);

					//check if we want to document this interface.
					ImplementsInfo implements = implementations[method.ToString()];
					if (implements == null) return;
				}

				writer.WriteStartElement("method");
				writer.WriteAttributeString("name", name);
				writer.WriteAttributeString("id", memberName);
				writer.WriteAttributeString("access", GetMethodAccessValue(method));
				writer.WriteAttributeString("contract", GetMethodContractValue(method));
				Type t = method.ReturnType;
				writer.WriteAttributeString("returnType", MemberID.GetTypeName(t));
				writer.WriteAttributeString("valueType", t.IsValueType.ToString().ToLower());

				if (inherited)
				{
					writer.WriteAttributeString("declaringType", MemberID.GetDeclaringTypeName(method));
				}

				if (overload > 0)
				{
					writer.WriteAttributeString("overload", overload.ToString());
				}

				if (!IsMemberSafe(method))
					writer.WriteAttributeString("unsafe", "true");

				if (hiding)
				{
					writer.WriteAttributeString("hiding", "true");
				}

				if (interfaceName != null)
				{
					writer.WriteAttributeString("interface", interfaceName);
				}

				if (inherited)
				{
					WriteInheritedDocumentation(writer, memberName, method.DeclaringType);
				}
				else
				{
					WriteMethodDocumentation(writer, memberName, method, true);
				}

				WriteCustomAttributes(writer, method);

				foreach (ParameterInfo parameter in method.GetParameters())
				{
					WriteParameter(writer, MemberID.GetMemberID(method), parameter);
				}

				if (implementations != null)
				{
					ImplementsInfo implements = implementations[method.ToString()];
					if (implements != null)
					{
						writer.WriteStartElement("implements");
						writer.WriteAttributeString("name", implements.InterfaceMethod.Name);
						writer.WriteAttributeString("id", MemberID.GetMemberID((MethodBase)implements.InterfaceMethod));
						writer.WriteAttributeString("interface", MemberDisplayName.GetMemberDisplayName(implements.InterfaceType));
						writer.WriteAttributeString("interfaceId", MemberID.GetMemberID(implements.InterfaceType));
						writer.WriteAttributeString("declaringType", implements.InterfaceType.FullName.Replace('+', '.'));
						writer.WriteEndElement();
					}
				}

				writer.WriteEndElement();
			}
		}

		private void WriteParameter(XmlWriter writer, string memberName, ParameterInfo parameter)
		{
			string direction = "in";
			bool isParamArray = false;

			if (parameter.ParameterType.IsByRef)
			{
				direction = parameter.IsOut ? "out" : "ref";
			}

			if (parameter.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
			{
				isParamArray = true;
			}

			writer.WriteStartElement("parameter");
			writer.WriteAttributeString("name", parameter.Name);
			
			Type t = parameter.ParameterType;
#if NET_2_0
			writer.WriteAttributeString("type", MemberID.GetTypeName(t, false));
#else
			writer.WriteAttributeString("type", MemberID.GetTypeName(t));
#endif
			writer.WriteAttributeString("valueType", t.IsValueType.ToString().ToLower());

			if (t.IsPointer)
				writer.WriteAttributeString("unsafe", "true");

			if (parameter.IsOptional)
			{
				writer.WriteAttributeString("optional", "true");
				if (parameter.DefaultValue != null)
				{
					writer.WriteAttributeString("defaultValue", parameter.DefaultValue.ToString());
				}
				else
				{
					//HACK: assuming this is only for VB syntax
					writer.WriteAttributeString("defaultValue", "Nothing");
				}
			}

			if (direction != "in")
			{
				writer.WriteAttributeString("direction", direction);
			}

			if (isParamArray)
			{
				writer.WriteAttributeString("isParamArray", "true");
			}

			WriteCustomAttributes(writer, parameter);

			writer.WriteEndElement();
		}

		private void WriteOperator(XmlWriter writer, MethodInfo method, int overload)
		{
			if (method != null)
			{
				string memberName = MemberID.GetMemberID(method);

				writer.WriteStartElement("operator");
				writer.WriteAttributeString("name", method.Name);
				writer.WriteAttributeString("id", memberName);
				writer.WriteAttributeString("access", GetMethodAccessValue(method));
				writer.WriteAttributeString("contract", GetMethodContractValue(method));
				Type t = method.ReturnType;
				writer.WriteAttributeString("returnType", MemberID.GetTypeName(t));
				writer.WriteAttributeString("valueType", t.IsValueType.ToString().ToLower());

				bool inherited = method.DeclaringType != method.ReflectedType;

				if (inherited)
				{
					writer.WriteAttributeString("declaringType", MemberID.GetDeclaringTypeName(method));
				}

				if (overload > 0)
				{
					writer.WriteAttributeString("overload", overload.ToString());
				}

				if (!IsMemberSafe(method))
					writer.WriteAttributeString("unsafe", "true");

				if (inherited)
				{
					WriteInheritedDocumentation(writer, memberName, method.DeclaringType);
				}
				else
				{
					WriteMethodDocumentation(writer, memberName, method, true);
				}

				WriteCustomAttributes(writer, method);

				foreach (ParameterInfo parameter in method.GetParameters())
				{
					WriteParameter(writer, MemberID.GetMemberID(method), parameter);
				}

				writer.WriteEndElement();
			}
		}


		#endregion

		#region IsMemberSafe
		private bool IsMemberSafe(FieldInfo field)
		{
			return!field.FieldType.IsPointer;
		}

		private bool IsMemberSafe(PropertyInfo property)
		{
			return!property.PropertyType.IsPointer;
		}

		private bool IsMemberSafe(MethodBase method)
		{
			foreach (ParameterInfo parameter in method.GetParameters())
			{
				if (parameter.GetType().IsPointer)
					return false;
			}
			return true;
		}

		private bool IsMemberSafe(MethodInfo method)
		{
			if (method.ReturnType.IsPointer)
				return false;

			return IsMemberSafe((MethodBase)method);
		}
		#endregion

		#region Get MemberID of base of inherited member
		//TODO: Refactor into get base member and then use MemberID to get ID

		/// <summary>Used by GetFullNamespaceName(MemberInfo member) functions to build
		/// up most of the /doc member name.</summary>
		/// <param name="type"></param>
		private string GetTypeNamespaceName(Type type)
		{
			return type.FullName.Replace('+', '.');
		}

		/// <summary>Derives the member name ID for the base of an inherited field.</summary>
		/// <param name="field">The field to derive the member name ID from.</param>
		/// <param name="declaringType">The declaring type.</param>
		private string GetMemberName(FieldInfo field, Type declaringType)
		{
			return "F:" + declaringType.FullName.Replace("+", ".") + "." + field.Name;
		}

		/// <summary>Derives the member name ID for an event. Used to match nodes in the /doc XML.</summary>
		/// <param name="eventInfo">The event to derive the member name ID from.</param>
		/// <param name="declaringType">The declaring type.</param>
		private string GetMemberName(EventInfo eventInfo, Type declaringType)
		{
			return "E:" + declaringType.FullName.Replace("+", ".") + 
				"." + eventInfo.Name.Replace('.', '#').Replace('+', '#');
		}

		/// <summary>Derives the member name ID for the base of an inherited property.</summary>
		/// <param name="property">The property to derive the member name ID from.</param>
		/// <param name="declaringType">The declaring type.</param>
		private string GetMemberName(PropertyInfo property, Type declaringType)
		{
			string memberID = MemberID.GetMemberID(property);

			//extract member type (T:, P:, etc.)
			string memberType = memberID.Substring(0, 2);

			//extract member name
			int i = memberID.IndexOf('(');
			string memberName;
			if (i > -1)
			{
				memberName = memberID.Substring(memberID.LastIndexOf('.', i) + 1);
			}
			else
			{
				memberName = memberID.Substring(memberID.LastIndexOf('.') + 1);
			}

			//the member id in the declaring type
			string key = memberType + GetTypeNamespaceName(declaringType) + "." + memberName;
			return key;
		}

		/// <summary>Derives the member name ID for the basse of an inherited member function.</summary>
		/// <param name="method">The method to derive the member name ID from.</param>
		/// <param name="declaringType">The declaring type.</param>
		private string GetMemberName(MethodBase method, Type declaringType)
		{
			string memberID = MemberID.GetMemberID(method);

			//extract member type (T:, P:, etc.)
			string memberType = memberID.Substring(0, 2);

			//extract member name
			int i = memberID.IndexOf('(');
			string memberName;
			if (i > -1)
			{
				memberName = memberID.Substring(memberID.LastIndexOf('.', i) + 1);
			}
			else
			{
				memberName = memberID.Substring(memberID.LastIndexOf('.') + 1);
			}

			//the member id in the declaring type
			string key = memberType + GetTypeNamespaceName(declaringType) + "." + memberName;
			return key;
		}

		#endregion


		#region Enumeration Values

		private string GetTypeAccessValue(Type type)
		{
			string result = "Unknown";

			switch (type.Attributes & TypeAttributes.VisibilityMask)
			{
				case TypeAttributes.Public : 
					result = "Public";
					break;
				case TypeAttributes.NotPublic : 
					result = "NotPublic";
					break;
				case TypeAttributes.NestedPublic : 
					result = "NestedPublic";
					break;
				case TypeAttributes.NestedFamily : 
					result = "NestedFamily";
					break;
				case TypeAttributes.NestedFamORAssem : 
					if (this.rep.DocumentProtectedInternalAsProtected)
					{
						result = "NestedFamily";
					}
					else
					{
						result = "NestedFamilyOrAssembly";
					}
					break;
				case TypeAttributes.NestedAssembly : 
					result = "NestedAssembly";
					break;
				case TypeAttributes.NestedFamANDAssem : 
					result = "NestedFamilyAndAssembly";
					break;
				case TypeAttributes.NestedPrivate : 
					result = "NestedPrivate";
					break;
			}

			return result;
		}

		private string GetFieldAccessValue(FieldInfo field)
		{
			string result = "Unknown";

			switch (field.Attributes & FieldAttributes.FieldAccessMask)
			{
				case FieldAttributes.Public : 
					result = "Public";
					break;
				case FieldAttributes.Family : 
					result = "Family";
					break;
				case FieldAttributes.FamORAssem : 
					if (this.rep.DocumentProtectedInternalAsProtected)
					{
						result = "Family";
					}
					else
					{
						result = "FamilyOrAssembly";
					}
					break;
				case FieldAttributes.Assembly : 
					result = "Assembly";
					break;
				case FieldAttributes.FamANDAssem : 
					result = "FamilyAndAssembly";
					break;
				case FieldAttributes.Private : 
					result = "Private";
					break;
				case FieldAttributes.PrivateScope : 
					result = "PrivateScope";
					break;
			}

			return result;
		}

		private string GetPropertyAccessValue(PropertyInfo property)
		{
			MethodInfo method;

			if (property.GetGetMethod(true) != null)
			{
				method = property.GetGetMethod(true);
			}
			else
			{
				method = property.GetSetMethod(true);
			}

			return GetMethodAccessValue(method);
		}

		private string GetMethodAccessValue(MethodBase method)
		{
			string result;

			switch (method.Attributes & MethodAttributes.MemberAccessMask)
			{
				case MethodAttributes.Public : 
					result = "Public";
					break;
				case MethodAttributes.Family : 
					result = "Family";
					break;
				case MethodAttributes.FamORAssem : 
					if (this.rep.DocumentProtectedInternalAsProtected)
					{
						result = "Family";
					}
					else
					{
						result = "FamilyOrAssembly";
					}
					break;
				case MethodAttributes.Assembly : 
					result = "Assembly";
					break;
				case MethodAttributes.FamANDAssem : 
					result = "FamilyAndAssembly";
					break;
				case MethodAttributes.Private : 
					result = "Private";
					break;
				case MethodAttributes.PrivateScope : 
					result = "PrivateScope";
					break;
				default : 
					result = "Unknown";
					break;
			}

			return result;
		}

		private string GetMethodContractValue(MethodBase method)
		{
			string result;
			MethodAttributes methodAttributes = method.Attributes;

			if ((methodAttributes & MethodAttributes.Static) > 0)
			{
				result = "Static";
			}
			else if ((methodAttributes & MethodAttributes.Abstract) > 0)
			{
				result = "Abstract";
			}
			else if ((methodAttributes & MethodAttributes.Final) > 0)
			{
				result = "Final";
			}
			else if ((methodAttributes & MethodAttributes.Virtual) > 0)
			{
				if ((methodAttributes & MethodAttributes.NewSlot) > 0)
				{
					result = "Virtual";
				}
				else
				{
					result = "Override";
				}
			}
			else
			{
				result = "Normal";
			}

			return result;
		}

		#endregion

		private StringCollection GetNamespaceNames(Type[] types)
		{
			StringCollection namespaceNames = new StringCollection();

			foreach (Type type in types)
			{
				if (namespaceNames.Contains(type.Namespace) == false)
				{
					namespaceNames.Add(type.Namespace);
				}
			}

			return namespaceNames;
		}


		#region Missing Documentation

		private void CheckForMissingSummaryAndRemarks(
			XmlWriter writer, 
			string memberName)
		{
			if (this.rep.ShowMissingSummaries)
			{
				bool bMissingSummary = true;
				string xmldoc = assemblyDocCache.GetDoc(memberName);

				if (xmldoc != null)
				{
					XmlTextReader reader = new XmlTextReader(xmldoc, XmlNodeType.Element, null);
					while (reader.Read()) 
					{
						if (reader.NodeType == XmlNodeType.Element) 
						{
							if (reader.Name == "summary") 
							{
								string summarydetails = reader.ReadInnerXml();
								if (summarydetails.Length > 0 && !summarydetails.Trim().StartsWith("Summary description for"))
								{
									bMissingSummary = false;
									break;
								}
							}
						}
					}
				}

				if (bMissingSummary)
				{
					WriteMissingDocumentation(writer, "summary", null, 
						"Missing <summary> documentation for " + memberName);
					//Debug.WriteLine("@@missing@@\t" + memberName);
				}
			}

			if (this.rep.ShowMissingRemarks)
			{
				bool bMissingRemarks = true;
				string xmldoc = assemblyDocCache.GetDoc(memberName);

				if (xmldoc != null)
				{
					XmlTextReader reader = new XmlTextReader(xmldoc, XmlNodeType.Element, null);
					while (reader.Read()) 
					{
						if (reader.NodeType == XmlNodeType.Element) 
						{
							if (reader.Name == "remarks")
							{
								string remarksdetails = reader.ReadInnerXml();
								if (remarksdetails.Length > 0)
								{
									bMissingRemarks = false;
									break;
								}
							}
						}
					}
				}

				if (bMissingRemarks)
				{
					WriteMissingDocumentation(writer, "remarks", null, 
						"Missing <remarks> documentation for " + memberName);
				}
			}
		}

		private void CheckForMissingParams(
			XmlWriter writer, 
			string memberName, 
			ParameterInfo[] parameters)
		{
			if (this.rep.ShowMissingParams)
			{
				string xmldoc = assemblyDocCache.GetDoc(memberName);
				foreach (ParameterInfo parameter in parameters)
				{
					bool bMissingParams = true;

					if (xmldoc != null)
					{
						XmlTextReader reader = new XmlTextReader(xmldoc, XmlNodeType.Element, null);
						while (reader.Read()) 
						{
							if (reader.NodeType == XmlNodeType.Element) 
							{
								if (reader.Name == "param") 
								{
									string name = reader.GetAttribute("name");
									if (name == parameter.Name)
									{
										string paramsdetails = reader.ReadInnerXml();
										if (paramsdetails.Length > 0)
										{ 
											bMissingParams = false;
											break; // we can stop if we locate what we are looking for
										}
									}
								}
							}
						}
					}

					if (bMissingParams)
					{
						WriteMissingDocumentation(writer, "param", parameter.Name, 
							"Missing <param> documentation for " + parameter.Name);
					}
				}
			}
		}

		private void CheckForMissingReturns(
			XmlWriter writer, 
			string memberName, 
			MethodInfo method)
		{
			if (this.rep.ShowMissingReturns && 
				!"System.Void".Equals(method.ReturnType.FullName))
			{
				string xmldoc = assemblyDocCache.GetDoc(memberName);
				bool bMissingReturns = true;

				if (xmldoc != null)
				{
					XmlTextReader reader = new XmlTextReader(xmldoc, XmlNodeType.Element, null);
					while (reader.Read()) 
					{
						if (reader.NodeType == XmlNodeType.Element) 
						{
							if (reader.Name == "returns") 
							{
								string returnsdetails = reader.ReadInnerXml();
								if (returnsdetails.Length > 0)
								{ 
									bMissingReturns = false;
									break; // we can stop if we locate what we are looking for
								}
							}
						}
					}
				}

				if (bMissingReturns)
				{
					WriteMissingDocumentation(writer, "returns", null, 
						"Missing <returns> documentation for " + memberName);
				}
			}
		}

		private void CheckForMissingValue(
			XmlWriter writer, 
			string memberName)
		{
			if (this.rep.ShowMissingValues)
			{
				string xmldoc = assemblyDocCache.GetDoc(memberName);
				bool bMissingValues = true;

				if (xmldoc != null)
				{
					XmlTextReader reader = new XmlTextReader(xmldoc, XmlNodeType.Element, null);
					while (reader.Read()) 
					{
						if (reader.NodeType == XmlNodeType.Element) 
						{
							if (reader.Name == "value") 
							{
								string valuesdetails = reader.ReadInnerXml();
								if (valuesdetails.Length > 0)
								{
									bMissingValues = false;
									break; // we can stop if we locate what we are looking for
								}
							}
						}
					}
				}

				if (bMissingValues)
				{
					WriteMissingDocumentation(writer, "values", null, 
						"Missing <values> documentation for " + memberName);
				}
			}
		}

		private void WriteMissingDocumentation(
			XmlWriter writer, 
			string element, 
			string name, 
			string message)
		{
			WriteStartDocumentation(writer);

			writer.WriteStartElement(element);

			if (name != null)
			{
				writer.WriteAttributeString("name", name);
			}

			writer.WriteStartElement("span");
			writer.WriteAttributeString("class", "missing");
			writer.WriteString(message);
			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		#endregion


		#region Write Documentation

		private bool didWriteStartDocumentation = false;

		private void WriteStartDocumentation(XmlWriter writer)
		{
			if (!didWriteStartDocumentation)
			{
				writer.WriteStartElement("documentation");
				didWriteStartDocumentation = true;
			}
		}

		private void WriteEndDocumentation(XmlWriter writer)
		{
			if (didWriteStartDocumentation)
			{
				writer.WriteEndElement();
				didWriteStartDocumentation = false;
			}
		}

		private void WriteSlashDocElements(XmlWriter writer, string memberName)
		{
			string temp = assemblyDocCache.GetDoc(memberName);
			if (temp != null)	
			{
				WriteStartDocumentation(writer);
				writer.WriteRaw(temp);
			}
		}

		private void WriteInheritedDocumentation(
			XmlWriter writer, 
			string memberName, 
			Type declaringType)
		{
#if NET_2_0
            if (declaringType.HasGenericArguments) declaringType = declaringType.GetGenericTypeDefinition();
#endif
			string summary = externalSummaryCache.GetSummary(memberName, declaringType);
			if (summary.Length > 0)
			{
				WriteStartDocumentation(writer);
				writer.WriteRaw(summary);
				WriteEndDocumentation(writer);
			}
		}

		private void WriteTypeDocumentation(
			XmlWriter writer, 
			string memberName, 
			Type type)
		{
			CheckForMissingSummaryAndRemarks(writer, memberName);
			WriteSlashDocElements(writer, memberName);
			WriteEndDocumentation(writer);
		}

		private void WriteDelegateDocumentation(
			XmlWriter writer, 
			string memberName, 
			Type type, 
			MethodInfo method)
		{
			CheckForMissingParams(writer, memberName, method.GetParameters());
			CheckForMissingReturns(writer, memberName, method);
			WriteTypeDocumentation(writer, memberName, type);
			WriteEndDocumentation(writer);
		}

		private void WriteEnumerationDocumentation(XmlWriter writer, string memberName)
		{
			CheckForMissingSummaryAndRemarks(writer, memberName);
			WriteSlashDocElements(writer, memberName);
			WriteEndDocumentation(writer);
		}

		//if the constructor has no parameters and no summary,
		//add a default summary text.
		private bool DoAutoDocumentConstructor(
			XmlWriter writer, 
			string memberName, 
			ConstructorInfo constructor)
		{
			if (rep.AutoDocumentConstructors)
			{		
				if (constructor.GetParameters().Length == 0)
				{
					string xmldoc = assemblyDocCache.GetDoc(memberName);
					bool bMissingSummary = true;

					if (xmldoc != null)
					{
						XmlTextReader reader = new XmlTextReader(xmldoc, XmlNodeType.Element, null);
						while (reader.Read()) 
						{
							if (reader.NodeType == XmlNodeType.Element) 
							{
								if (reader.Name == "summary") 
								{
									string summarydetails = reader.ReadInnerXml();
									if (summarydetails.Length > 0 && !summarydetails.Trim().StartsWith("Summary description for"))
									{ 
										bMissingSummary = false;
									}
								}
							}
						}
					}

					if (bMissingSummary)
					{
						WriteStartDocumentation(writer);
						writer.WriteStartElement("summary");
						if (constructor.IsStatic)
						{
							writer.WriteString("Initializes the static fields of the ");
						}
						else
						{
							writer.WriteString("Initializes a new instance of the ");
						}
						writer.WriteStartElement("see");
						writer.WriteAttributeString("cref", MemberID.GetMemberID(constructor.DeclaringType));
						writer.WriteEndElement();
						writer.WriteString(" class.");
						writer.WriteEndElement();
						return true;
					}
				}
			}
			return false;
		}

		private void WriteConstructorDocumentation(
			XmlWriter writer, 
			string memberName, 
			ConstructorInfo constructor)
		{
			if (!DoAutoDocumentConstructor(writer, memberName, constructor))
			{
				CheckForMissingSummaryAndRemarks(writer, memberName);
				CheckForMissingParams(writer, memberName, constructor.GetParameters());
			}
			WriteSlashDocElements(writer, memberName);
			WriteEndDocumentation(writer);
		}

		private void WriteFieldDocumentation(
			XmlWriter writer, 
			string memberName, 
			Type type)
		{
			if (!CheckForPropertyBacker(writer, memberName, type))
			{
				CheckForMissingSummaryAndRemarks(writer, memberName);
			}
			WriteSlashDocElements(writer, memberName);
			WriteEndDocumentation(writer);
		}

		private void WritePropertyDocumentation(
			XmlWriter writer, 
			string memberName, 
			PropertyInfo property, 
			bool writeMissing)
		{
			if (writeMissing)
			{
				CheckForMissingSummaryAndRemarks(writer, memberName);
				CheckForMissingParams(writer, memberName, GetIndexParameters(property));
				CheckForMissingValue(writer, memberName);
			}
			WriteSlashDocElements(writer, memberName);
			WriteEndDocumentation(writer);
		}

		private void WriteMethodDocumentation(
			XmlWriter writer, 
			string memberName, 
			MethodInfo method, 
			bool writeMissing)
		{
			if (writeMissing)
			{
				CheckForMissingSummaryAndRemarks(writer, memberName);
				CheckForMissingParams(writer, memberName, method.GetParameters());
				CheckForMissingReturns(writer, memberName, method);
			}
			WriteSlashDocElements(writer, memberName);
			WriteEndDocumentation(writer);
		}

		private void WriteEventDocumentation(
			XmlWriter writer, 
			string memberName, 
			bool writeMissing)
		{
			if (writeMissing)
			{
				CheckForMissingSummaryAndRemarks(writer, memberName);
			}
			WriteSlashDocElements(writer, memberName);
			WriteEndDocumentation(writer);
		}

		#endregion


		#region Property Backers

		/// <summary>
		/// This checks whether a field is a property backer, meaning
		/// it stores the information for the property.
		/// </summary>
		/// <remarks>
		/// <para>This takes advantage of the fact that most people
		/// have a simple convention for the names of the fields
		/// and the properties that they back.
		/// If the field doesn't have a summary already, and it
		/// looks like it backs a property, and the BaseDocumenterConfig
		/// property is set appropriately, then this adds a
		/// summary indicating that.</para>
		/// <para>Note that this design will call multiple fields the 
		/// backer for a single property.</para>
		/// <para/>This also will call a public field a backer for a
		/// property, when typically that wouldn't be the case.
		/// </remarks>
		/// <param name="writer">The XmlWriter to write to.</param>
		/// <param name="memberName">The full name of the field.</param>
		/// <param name="type">The Type which contains the field
		/// and potentially the property.</param>
		/// <returns>True only if a property backer is auto-documented.</returns>
		private bool CheckForPropertyBacker(
			XmlWriter writer, 
			string memberName, 
			Type type)
		{
			if (!this.rep.AutoPropertyBackerSummaries) return false;

			// determine if field is non-public
			// (because public fields are probably not backers for properties)
			bool isNonPublic = true; // stubbed out for now

			//check whether or not we have a valid summary
			bool isMissingSummary = true;
			string xmldoc = assemblyDocCache.GetDoc(memberName);
			if (xmldoc != null)
			{
				XmlTextReader reader = new XmlTextReader(xmldoc, XmlNodeType.Element, null);
				while (reader.Read()) 
				{
					if (reader.NodeType == XmlNodeType.Element) 
					{
						if (reader.Name == "summary") 
						{
							string summarydetails = reader.ReadInnerXml();
							if (summarydetails.Length > 0 && !summarydetails.Trim().StartsWith("Summary description for"))
							{
								isMissingSummary = false;
							}
						}
					}
				}
			}

			// only do this if there is no summary already
			if (isMissingSummary && isNonPublic)
			{
				// find the property (if any) that this field backs

				// generate the possible property names that this could back
				// so far have: property Name could be backed by _Name or name
				// but could be other conventions
				string[] words = memberName.Split('.');
				string fieldBaseName = words[words.Length - 1];
				string firstLetter = fieldBaseName.Substring(0, 1);
				string camelCasePropertyName = firstLetter.ToUpper() 
					+ fieldBaseName.Remove(0, 1);
				string usPropertyName = fieldBaseName.Replace("_", "");

				// find it
				PropertyInfo propertyInfo;

				if (((propertyInfo = FindProperty(camelCasePropertyName, 
					type)) != null)
					|| ((propertyInfo = FindProperty(usPropertyName, 
					type)) != null))
				{
					WritePropertyBackerDocumentation(writer, "summary", 
						propertyInfo);
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Find a particular property of the specified type, by name.
		/// Return the PropertyInfo for it.
		/// </summary>
		/// <param name="expectedPropertyName">The name of the property to
		/// find.</param>
		/// <param name="type">The type in which to search for 
		/// the property.</param>
		/// <returns>PropertyInfo - The property info, or null for 
		/// not found.</returns>
		private PropertyInfo FindProperty(string expectedPropertyName, 
			Type type)
		{
			const BindingFlags bindingFlags = 
					  BindingFlags.Instance | 
					  BindingFlags.Static | 
					  BindingFlags.Public | 
					  BindingFlags.NonPublic;

			PropertyInfo[] properties = type.GetProperties(bindingFlags);
			foreach (PropertyInfo property in properties)
			{
				if (property.Name.Equals(expectedPropertyName))
				{
					MethodInfo getMethod = property.GetGetMethod(true);
					MethodInfo setMethod = property.GetSetMethod(true);

					bool hasGetter = (getMethod != null) && MustDocumentMethod(getMethod);
					bool hasSetter = (setMethod != null) && MustDocumentMethod(setMethod);

					if ((hasGetter || hasSetter) && !IsAlsoAnEvent(property))
					{
						return (property);
					}
				}
			}

			return (null);
		}

		/// <summary>
		/// Write xml info for a property's backer field to the specified writer.
		/// This writes a string with a link to the property.
		/// </summary>
		/// <param name="writer">The XmlWriter to write to.</param>
		/// <param name="element">The field which backs the property.</param>
		/// <param name="property">The property backed by the field.</param>
		private void WritePropertyBackerDocumentation(
			XmlWriter writer, 
			string element, 
			PropertyInfo property)
		{
			string propertyName = property.Name;
			string propertyId = "P:" + property.DeclaringType.FullName + "."
				+ propertyName;

			WriteStartDocumentation(writer);
			writer.WriteStartElement(element);
			writer.WriteRaw("Backer for property <see cref=\"" 
				+ propertyId + "\">" + property.Name + "</see>");
			writer.WriteEndElement();
		}

		#endregion


		#region PreReflectionProcess

		private void PreReflectionProcess()
		{
			PreLoadXmlDocumentation();
			BuildXrefs();
		}

		private void PreLoadXmlDocumentation()
		{
			if (assemblyDocCache == null)	assemblyDocCache = new AssemblyXmlDocCache();
			//preload all xml documentation
			foreach (string xmlDocFilename in this.rep.XmlDocFileNames)
			{
				externalSummaryCache.AddXmlDoc(xmlDocFilename);
				assemblyDocCache.CacheDocFile(xmlDocFilename);
			}
		}
		
		private void BuildXrefs()
		{
			//build derived members and implementing types xrefs.
			foreach (string assemblyFileName in this.rep.AssemblyFileNames)
			{
				//attempt to load the assembly
				Assembly assembly = assemblyLoader.LoadAssembly(assemblyFileName);

				// loop through all types in assembly
				foreach (Type type in assembly.GetTypes())
				{
					if (MustDocumentType(type))
					{
						BuildDerivedMemberXref(type);
						BuildDerivedInterfaceXref(type);
						string friendlyNamespaceName;
						if (type.Namespace == null)
						{
							friendlyNamespaceName = "(global)";
						}
						else
						{
							friendlyNamespaceName = type.Namespace;
						}
						BuildNamespaceHierarchy(friendlyNamespaceName, type);
						notEmptyNamespaces[friendlyNamespaceName] = null;
					}
				}
			}
		}
		
		private void BuildDerivedMemberXref(Type type)
		{
			if (type.BaseType != null && 
				MustDocumentType(type.BaseType)) // we don't care about undocumented types
			{
				derivedTypes.Add(type.BaseType, type);
			}
		}

		private void BuildNamespaceHierarchy(string namespaceName, Type type)
		{
			if ((type.BaseType != null) && 
				MustDocumentType(type.BaseType)) // we don't care about undocumented types
			{
				namespaceHierarchies.Add(namespaceName, type.BaseType, type);
				BuildNamespaceHierarchy(namespaceName, type.BaseType);
			}
			if (type.IsInterface)
			{
				namespaceHierarchies.Add(namespaceName, typeof(System.Object), type /*.BaseType*/);
			}
			//build a collection of the base type's interfaces
			//to determine which have been inherited
			StringCollection interfacesOnBase = new StringCollection();
			if (type.BaseType != null)
			{
				foreach (Type baseInterfaceType in type.BaseType.GetInterfaces())
				{
					interfacesOnBase.Add(MemberID.GetMemberID(baseInterfaceType));
				}
			}
			foreach (Type interfaceType in type.GetInterfaces())
			{
				if (MustDocumentType(interfaceType))
				{
					if (!interfacesOnBase.Contains(MemberID.GetMemberID(interfaceType)))
					{
						baseInterfaces.Add(type, interfaceType);
					}
				}
			}
		}

		private void BuildDerivedInterfaceXref(Type type)
		{
			foreach (Type interfaceType in type.GetInterfaces())
			{
				if (MustDocumentType(interfaceType)) // we don't care about undocumented types
				{
					if (type.IsInterface)
					{
						derivedTypes.Add(interfaceType, type);
					}
					else
					{
						interfaceImplementingTypes.Add(interfaceType, type);
					}
				}
			}
		}

		#endregion


		#region Write Hierarchies

		private void WriteDerivedTypes(XmlWriter writer, Type type)
		{
			foreach (Type derived in derivedTypes.GetDerivedTypes(type))
			{
				writer.WriteStartElement("derivedBy");
				writer.WriteAttributeString("id", MemberID.GetMemberID(derived));
				writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(derived));
				writer.WriteAttributeString("namespace", derived.Namespace);
				writer.WriteEndElement();
			}
		}

		private void WriteInterfaceImplementingTypes(XmlWriter writer, Type type)
		{
			foreach (Type implementingType in interfaceImplementingTypes.GetDerivedTypes(type))
			{
				writer.WriteStartElement("implementedBy");
				writer.WriteAttributeString("id", MemberID.GetMemberID(implementingType));
				writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(implementingType));
				writer.WriteAttributeString("namespace", implementingType.Namespace);
				writer.WriteEndElement();
			}
		}
		

		private void WriteNamespaceHierarchies(XmlWriter writer)
		{
			writer.WriteStartElement("namespaceHierarchies");
			foreach (string namespaceName in namespaceHierarchies.DefinedNamespaces)
			{
				WriteNamespaceTypeHierarchy(writer, namespaceName);
			}
			writer.WriteEndElement();
		}
		
		private void WriteNamespaceTypeHierarchy(XmlWriter writer, string namespaceName)
		{
			//get all base types from which members of this namespace are derived
			TypeHierarchy derivedTypesCollection = namespaceHierarchies.GetDerivedTypesCollection(namespaceName);
			if (derivedTypesCollection != null)
			{
				//we will always start the hierarchy with System.Object (hopefully for obvious reasons)
				writer.WriteStartElement("namespaceHierarchy");
				writer.WriteAttributeString("name", namespaceName);
				WriteTypeHierarchy(writer, derivedTypesCollection, typeof(System.Object));
				writer.WriteEndElement();
			}
		}
		
		private void WriteTypeHierarchy(XmlWriter writer, TypeHierarchy derivedTypes, Type type)
		{
			writer.WriteStartElement("hierarchyType");
			writer.WriteAttributeString("id", MemberID.GetMemberID(type));
			writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(type));
			writer.WriteAttributeString("namespace", type.Namespace);
			ArrayList interfaces = baseInterfaces.GetDerivedTypes(type);
			if (interfaces.Count > 0)
			{
				writer.WriteStartElement("hierarchyInterfaces");
				foreach (Type baseInterfaceType in interfaces)
				{
					writer.WriteStartElement("hierarchyInterface");
					writer.WriteAttributeString("id", MemberID.GetMemberID(baseInterfaceType));
					writer.WriteAttributeString("displayName", MemberDisplayName.GetMemberDisplayName(baseInterfaceType));
					writer.WriteAttributeString("namespace", baseInterfaceType.Namespace);
					writer.WriteAttributeString("fullName", baseInterfaceType.FullName);
					writer.WriteEndElement();
				}
				writer.WriteEndElement();
			}
			ArrayList childTypesList = derivedTypes.GetDerivedTypes(type);
			foreach (Type childType in childTypesList)
			{
				WriteTypeHierarchy(writer, derivedTypes, childType);
			}
			writer.WriteEndElement();
		}

		#endregion

		private void TraceErrorOutput(string message, Exception ex)
		{
			Trace.WriteLine("[WARNING] " + message);
			if (ex != null)
			{
				Exception tempEx = ex;
				do
				{
					Trace.WriteLine("-> " + tempEx.GetType().ToString() + ":" + ex.Message);
					tempEx = tempEx.InnerException;
				} while (tempEx != null);
				Trace.WriteLine(ex.StackTrace);
			}
		}

		private AssemblyLoader SetupAssemblyLoader()
		{
			AssemblyLoader assemblyLoader = new AssemblyLoader(rep.ReferencePaths);

			assemblyLoader.Install();

			return (assemblyLoader);
		}


		#region ImplementsInfo

		private class ImplementsInfo
		{
			public Type TargetType;
			public MemberInfo TargetMethod;
			public Type InterfaceType;
			public MemberInfo InterfaceMethod;
		}
		private class ImplementsCollection
		{
			private Hashtable data;
			public ImplementsCollection()
			{
				data = new Hashtable(15); // give it an initial capacity...
			}
			public ImplementsInfo this[string name]
			{
				get { return (ImplementsInfo)data[name]; }
				set { data[name] = value; }
			}
		}

		#endregion

	
	}
}
