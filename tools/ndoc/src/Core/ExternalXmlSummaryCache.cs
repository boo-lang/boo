// Copyright (C) 2004  Kevin Downs
// Parts Copyright (c) 2002 Jean-Claude Manoli
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
using System.IO;
using System.Collections;
using System.Globalization;
using System.Xml;
using System.Reflection;
using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace NDoc.Core.Reflection
{
	/// <summary>
	/// Caches XML summaries.
	/// </summary>
	internal class ExternalXmlSummaryCache
	{
		private Hashtable cachedDocs;
		private Hashtable summaries;
		private string localizationLanguage;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExternalXmlSummaryCache" />
		/// class.
		/// </summary>
		public ExternalXmlSummaryCache(string localizationLanguage)
		{
			Flush();
			this.localizationLanguage = localizationLanguage;
		}

		/// <summary>
		/// Flushes the <see cref="ExternalXmlSummaryCache" />.
		/// </summary>
		public void Flush()
		{
			cachedDocs = new Hashtable();
			summaries = new Hashtable();
		}

		/// <summary>
		/// Adds given XML document to the summary cache.
		/// </summary>
		/// <param name="xmlFileName">The filename of XML document to cache.</param>
		public void AddXmlDoc(string xmlFileName)
		{
			int start = Environment.TickCount;

			XmlTextReader reader = null;
			try
			{
				reader = new XmlTextReader(xmlFileName);
				CacheSummaries(reader);
			}
			finally
			{
				if (reader != null) reader.Close();
			}
			Debug.WriteLine("Cached doc : " + Path.GetFileName(xmlFileName) + " (" + ((Environment.TickCount - start) / 1000.0).ToString() + " sec.)");
		}

		/// <summary>
		/// Gets the xml documentation for the assembly of the specified type.
		/// </summary>
		public void GetXmlFor(Type type)
		{
			string searchedDoc = (string)cachedDocs[type.Assembly.FullName];

			if (searchedDoc == null)
			{
				//Debug.WriteLine("Attempting to locate XML docs for " + type.Assembly.FullName);
				Type t = Type.GetType(type.AssemblyQualifiedName);
				if (t != null)
				{
					Assembly a = t.Assembly;
					string assemblyPath = a.Location;
						
					if (assemblyPath.Length > 0)
					{
						string docPath = Path.ChangeExtension(assemblyPath, ".xml");

						//if not found, try loading __AssemblyInfo__.ini
						if (!File.Exists(docPath))
						{
							string infoPath = Path.Combine(
								Path.GetDirectoryName(docPath), "__AssemblyInfo__.ini");
							docPath = null;

							if (File.Exists(infoPath))
							{
								//Debug.WriteLine("Loading __AssemblyInfo__.ini.");
								TextReader reader = new StreamReader(infoPath);
								string line;
								try
								{
									while ((line = reader.ReadLine()) != null)
									{
										if (line.StartsWith("URL=file:///"))
										{
											docPath = Path.ChangeExtension(line.Substring(12), ".xml");
											break;
										}
									}
								}
								finally
								{
									reader.Close();
								}
							}
						}

						//TODO: search in the mono lib folder, if they ever give us the xml documentation
						// If still not found, try locating the assembly in the Framework folder
						if (!RunningOnMono && (docPath == null || !File.Exists(docPath)))
						{
#if (NET_1_0)
							FileVersionInfo version = FileVersionInfo.GetVersionInfo(assemblyPath);
							string stringVersion = string.Format(
								"v{0}.{1}.{2}", 
								version.FileMajorPart, 
								version.FileMinorPart, 
								version.FileBuildPart);
							string frameworkPath = this.GetDotnetFrameworkPath(stringVersion);
#else
							string frameworkPath = this.GetDotnetFrameworkPath(a.ImageRuntimeVersion);
#endif
							if (frameworkPath != null)
							{
								string localizedFrameworkPath = Path.Combine(frameworkPath, localizationLanguage);
								if (Directory.Exists(localizedFrameworkPath))
								{
									docPath = Path.Combine(localizedFrameworkPath, a.GetName().Name + ".xml");
								}
								if ((docPath == null) || (!File.Exists(docPath)))
								{
									docPath = Path.Combine(frameworkPath, a.GetName().Name + ".xml");
								}
							}
						}

						if ((docPath != null) && (File.Exists(docPath)))
						{
							Debug.WriteLine("Docs found : " + docPath);
							AddXmlDoc(docPath);
							searchedDoc = docPath;
						}
					}
				}


				//if the doc was still not found, create an empty document filename
				if (searchedDoc == null)
				{
					Trace.WriteLine("XML Doc not found for " + type.Assembly.FullName);
					searchedDoc = "";
				}
				//cache the document path
				cachedDocs.Add(type.Assembly.FullName, searchedDoc);
			}
		}

		/// <summary>
		/// Caches summaries for all members in XML documentation file.
		/// </summary>
		/// <param name="reader">XmlTextReader for XML Documentation</param>
		/// <remarks>If a member does not have a summary, a zero-length string is stored instead.</remarks>
		private void CacheSummaries(XmlTextReader reader)
		{
			object oMember = reader.NameTable.Add("member");
			object oSummary = reader.NameTable.Add("summary");

			reader.MoveToContent();

			string MemberID = "";
			string Summary = "";
			while (reader.Read()) 
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element : 

						if (reader.Name.Equals(oMember)) 
						{
							MemberID = reader.GetAttribute("name");
							Summary = "";
						}      
						if (reader.Name.Equals(oSummary)) 
						{
							Summary = reader.ReadInnerXml();
							Summary = PreprocessDoc(MemberID, Summary);
						}
						break;

					case XmlNodeType.EndElement : 
 
						if (reader.Name.Equals(oMember)) 
						{
							if (!summaries.ContainsKey(MemberID))
							{
								summaries.Add(MemberID, Summary);
							}
						}
						break;

					default : 
						break;
				}
			}
		}

		/// <summary>
		/// Preprocess documentation before placing it in the cache.
		/// </summary>
		/// <param name="id">Member name 'id' to which the docs belong</param>
		/// <param name="doc">A string containing the members documentation</param>
		/// <returns>processed doc string</returns>
		private string PreprocessDoc(string id, string doc)
		{
			//create an XmlDocument containg the memeber's documentation
			XmlTextReader reader=new XmlTextReader(new StringReader("<root>" + doc + "</root>"));
			reader.WhitespaceHandling=WhitespaceHandling.All;
			
			XmlDocument xmldoc = new XmlDocument();
			xmldoc.PreserveWhitespace=true;
			xmldoc.Load(reader);
 
			//
			CleanupNodes(xmldoc.DocumentElement.ChildNodes);
			//
			ProcessSeeLinks(id, xmldoc.DocumentElement.ChildNodes);
			return xmldoc.DocumentElement.InnerXml;
		}

		/// <summary>
		/// strip out redundant newlines and spaces from documentation.
		/// </summary>
		/// <param name="nodes">list of nodes</param>
		private void CleanupNodes(XmlNodeList nodes)
		{
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					CleanupNodes(node.ChildNodes);

					// Trim attribute values...
					foreach(XmlNode attr in node.Attributes)
					{
						attr.Value=attr.Value.Trim();
					}
				}
				if (node.NodeType == XmlNodeType.Text)
				{
					node.Value = ((string)node.Value).Replace("\t", "    ").Replace("\n", " ").Replace("\r", " ").Replace("        ", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ");
				}
			}
		}

		/// <summary>
		/// Add 'nolink' attributes to self referencing or duplicate see tags.
		/// </summary>
		/// <param name="id">current member name 'id'</param>
		/// <param name="nodes">list of top-level nodes</param>
		/// <remarks>
		/// </remarks>
		private void ProcessSeeLinks(string id, XmlNodeList nodes)
		{
			foreach (XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element) 
				{
					Hashtable linkTable=null;
					MarkupSeeLinks(ref linkTable, id, node);
				}
			}
		}

		/// <summary>
		/// Search tags for duplicate or self-referencing see links.
		/// </summary>
		/// <param name="linkTable">A table of previous links.</param>
		/// <param name="id">current member name 'id'</param>
		/// <param name="node">an Xml Node containing a doc tag</param>
		private void MarkupSeeLinks(ref Hashtable linkTable, string id, XmlNode node)
		{
			if (node.LocalName=="see")
			{
				//we will only do this for crefs
				XmlAttribute cref = node.Attributes["cref"];
				if (cref !=null)
				{
					if (cref.Value==id) //self referencing tag
					{
						XmlAttribute dup = node.OwnerDocument.CreateAttribute("nolink");
						dup.Value="true";
						node.Attributes.Append(dup);
					}
					else
					{
						if (linkTable==null)
						{
							//assume an resonable initial table size,
							//so we don't have to resize often.
							linkTable = new Hashtable(16);
						}
						if (linkTable.ContainsKey(cref.Value))
						{
							XmlAttribute dup = node.OwnerDocument.CreateAttribute("nolink");
							dup.Value="true";
							node.Attributes.Append(dup);
						}
						else
						{
							linkTable.Add(cref.Value,null);
						}
					}
				}
			}
				
			//search this tags' children
			foreach (XmlNode childnode in node.ChildNodes)
			{
				if (childnode.NodeType == XmlNodeType.Element) 
				{
					MarkupSeeLinks(ref linkTable, id, childnode);
				}
			}
		}


		/// <summary>
		/// Returns the original summary for a member inherited from a specified type. 
		/// </summary>
		/// <param name="memberID">The member ID to lookup.</param>
		/// <param name="declaringType">The type that declares that member.</param>
		/// <returns>The summary xml.  If not found, returns an zero length string.</returns>
		public string GetSummary(string memberID, Type declaringType)
		{
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

			//the member id in the declaring assembly
			string key = memberType + declaringType.FullName.Replace("+", ".") + "." + memberName;

			//check the summaries cache first
			string summary = (string)summaries[key];

			if (summary == null)
			{
				//lookup the xml document
				GetXmlFor(declaringType);

				//the summary should now be cached (if it exists!)
				//so lets have another go at getting it...
				summary = (string)summaries[key];

				//if no summary was not found, create an blank one
				if (summary == null)
				{
					//Debug.WriteLine("#NoSummary#\t" + key); 
					summary = "";
					//cache the blank so we don't search for it again
					summaries.Add(key, summary);
				}

			}
			return "<summary>" + summary + "</summary>";
		}

		private string GetDotnetFrameworkPath(string version)
		{
			using (RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework"))
			{
				if (regKey == null)
					return null;

				string installRoot = regKey.GetValue("InstallRoot") as string;

				if (installRoot == null)
					return null;

				return Path.Combine(installRoot, version);
			}
		}

		private static bool RunningOnMono
		{
			get
			{
				// check a class in mscorlib to determine if we're running on Mono
				return (Type.GetType ("System.MonoType", false) != null);
			}
		}
	}
}
