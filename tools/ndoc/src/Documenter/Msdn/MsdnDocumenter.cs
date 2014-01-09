// MsdnDocumenter.cs - a MSDN-like documenter
// Copyright (C) 2001  Kral Ferch, Jason Diamond
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Globalization;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.Msdn
{
	/// <summary>The MsdnDocumenter class.</summary>
	public class MsdnDocumenter : BaseReflectionDocumenter
	{
		enum WhichType
		{
			Class,
			Interface,
			Structure,
			Enumeration,
			Delegate,
			Unknown
		};

		private HtmlHelp htmlHelp;

		private XmlDocument xmlDocumentation;
		private XPathDocument xpathDocument;

		private Hashtable lowerCaseTypeNames;
		private Hashtable mixedCaseTypeNames;
		private StringDictionary fileNames;
		private StringDictionary elemNames;
		private MsdnXsltUtilities utilities;

		private StyleSheetCollection stylesheets;

		private Encoding currentFileEncoding;

		private ArrayList documentedNamespaces;

		private Workspace workspace;

		/// <summary>
		/// Initializes a new instance of the <see cref="MsdnDocumenter" />
		/// class.
		/// </summary>
		public MsdnDocumenter( MsdnDocumenterConfig config ) : base( config )
		{
			lowerCaseTypeNames = new Hashtable();
			lowerCaseTypeNames.Add(WhichType.Class, "class");
			lowerCaseTypeNames.Add(WhichType.Interface, "interface");
			lowerCaseTypeNames.Add(WhichType.Structure, "structure");
			lowerCaseTypeNames.Add(WhichType.Enumeration, "enumeration");
			lowerCaseTypeNames.Add(WhichType.Delegate, "delegate");

			mixedCaseTypeNames = new Hashtable();
			mixedCaseTypeNames.Add(WhichType.Class, "Class");
			mixedCaseTypeNames.Add(WhichType.Interface, "Interface");
			mixedCaseTypeNames.Add(WhichType.Structure, "Structure");
			mixedCaseTypeNames.Add(WhichType.Enumeration, "Enumeration");
			mixedCaseTypeNames.Add(WhichType.Delegate, "Delegate");

			fileNames = new StringDictionary();
			elemNames = new StringDictionary();
		}

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override string MainOutputFile 
		{ 
			get 
			{
				if ((MyConfig.OutputTarget & OutputType.HtmlHelp) > 0)
				{
					return Path.Combine(MyConfig.OutputDirectory, 
						MyConfig.HtmlHelpName + ".chm");
				}
				else
				{
					return Path.Combine(MyConfig.OutputDirectory, "index.html");
				}
			} 
		}

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override string CanBuild(Project project, bool checkInputOnly)
		{
			string result = base.CanBuild(project, checkInputOnly); 
			if (result != null)
			{
				return result;
			}

			string AdditionalContentResourceDirectory = MyConfig.AdditionalContentResourceDirectory;
			if ( AdditionalContentResourceDirectory.Length != 0 && !Directory.Exists( AdditionalContentResourceDirectory ) )
				return string.Format( "The Additional Content Resource Directory {0} could not be found", AdditionalContentResourceDirectory );

			string ExtensibilityStylesheet = MyConfig.ExtensibilityStylesheet;
			if ( ExtensibilityStylesheet.Length != 0 && !File.Exists( ExtensibilityStylesheet ) )
				return string.Format( "The Extensibility Stylesheet file {0} could not be found", ExtensibilityStylesheet );

			if (checkInputOnly) 
			{
				return null;
			}

			string path = Path.Combine(MyConfig.OutputDirectory, 
				MyConfig.HtmlHelpName + ".chm");

			string temp = Path.Combine(MyConfig.OutputDirectory, "~chm.tmp");

			try
			{

				if (File.Exists(path))
				{
					//if we can move the file, then it is not open...
					File.Move(path, temp);
					File.Move(temp, path);
				}
			}
			catch (Exception)
			{
				result = "The compiled HTML Help file is probably open.\nPlease close it and try again.";
			}

			return result;
		}

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override void Build(Project project)
		{
			try
			{
				OnDocBuildingStep(0, "Initializing...");

				//Get an Encoding for the current LangID
				CultureInfo ci = new CultureInfo(MyConfig.LangID);
				currentFileEncoding = Encoding.GetEncoding(ci.TextInfo.ANSICodePage);

				// the workspace class is responsible for maintaining the outputdirectory
				// and compile intermediate locations
				workspace = new MsdnWorkspace( Path.GetFullPath( MyConfig.OutputDirectory ) );
				workspace.Clean();
				workspace.Prepare();

				// Write the embedded css files to the html output directory
				EmbeddedResources.WriteEmbeddedResources(
					this.GetType().Module.Assembly,
					"NDoc.Documenter.Msdn.css",
					workspace.WorkingDirectory);

				// Write the embedded icons to the html output directory
				EmbeddedResources.WriteEmbeddedResources(
					this.GetType().Module.Assembly,
					"NDoc.Documenter.Msdn.images",
					workspace.WorkingDirectory);

				// Write the embedded scripts to the html output directory
				EmbeddedResources.WriteEmbeddedResources(
					this.GetType().Module.Assembly,
					"NDoc.Documenter.Msdn.scripts",
					workspace.WorkingDirectory);

				if ( ((string)MyConfig.AdditionalContentResourceDirectory).Length > 0 )			
					workspace.ImportContentDirectory( MyConfig.AdditionalContentResourceDirectory );

				// Write the external files (FilesToInclude) to the html output directory

				foreach( string srcFilePattern in MyConfig.FilesToInclude.Split( '|' ) )
				{
					if ((srcFilePattern == null) || (srcFilePattern.Length == 0))
						continue;

					string path = Path.GetDirectoryName(srcFilePattern);
					string pattern = Path.GetFileName(srcFilePattern);
 
					// Path.GetDirectoryName can return null in some cases.
					// Treat this as an empty string.
					if (path == null)
						path = string.Empty;
 
					// Make sure we have a fully-qualified path name
					if (!Path.IsPathRooted(path))
						path = Path.Combine(Environment.CurrentDirectory, path);
 
					// Directory.GetFiles does not accept null or empty string
					// for the searchPattern parameter. When no pattern was
					// specified, assume all files (*) are wanted.
					if ((pattern == null) || (pattern.Length == 0))
						pattern = "*";
 
					foreach(string srcFile in Directory.GetFiles(path, pattern))
					{
						string dstFile = Path.Combine(workspace.WorkingDirectory, Path.GetFileName(srcFile));
						File.Copy(srcFile, dstFile, true);
						File.SetAttributes(dstFile, FileAttributes.Archive);
					}
				}
				OnDocBuildingStep(10, "Merging XML documentation...");

				// Will hold the name of the file name containing the XML doc
				string tempFileName = null;

				try 
				{
					// determine temp file name
					tempFileName = Path.GetTempFileName();
					// Let the Documenter base class do it's thing.
					MakeXmlFile(project, tempFileName);

					// Load the XML documentation into DOM and XPATH doc.
					using (FileStream tempFile = File.Open(tempFileName, FileMode.Open, FileAccess.Read)) 
					{
						FilteringXmlTextReader fxtr = new FilteringXmlTextReader(tempFile);
						xmlDocumentation = new XmlDocument();
						xmlDocumentation.Load(fxtr);

						tempFile.Seek(0,SeekOrigin.Begin);
						XmlTextReader reader = new XmlTextReader(tempFile);
						xpathDocument = new XPathDocument(reader, XmlSpace.Preserve);
					}
				}
				finally
				{
					if (tempFileName != null && File.Exists(tempFileName)) 
					{
						File.Delete(tempFileName);
					}
				}

				XmlNodeList typeNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace/*[name()!='documentation']");
				if (typeNodes.Count == 0)
				{
					throw new DocumenterException("There are no documentable types in this project.\n\nAny types that exist in the assemblies you are documenting have been excluded by the current visibility settings.\nFor example, you are attempting to document an internal class, but the 'DocumentInternals' visibility setting is set to False.\n\nNote: C# defaults to 'internal' if no accessibilty is specified, which is often the case for Console apps created in VS.NET...");
				}

				XmlNodeList namespaceNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace");
				int[] indexes = SortNodesByAttribute(namespaceNodes, "name");

				XmlNode defaultNamespace = namespaceNodes[indexes[0]];;

				string defaultNamespaceName = (string)defaultNamespace.Attributes["name"].Value;
				string defaultTopic = defaultNamespaceName + ".html";

				// setup for root page
				string rootPageFileName = null;
				string rootPageTOCName = "Overview";

				if ((MyConfig.RootPageFileName != null) && (MyConfig.RootPageFileName != string.Empty))
				{
					rootPageFileName = MyConfig.RootPageFileName;
					defaultTopic = "default.html";

					// what to call the top page in the table of contents?
					if ((MyConfig.RootPageTOCName != null) && (MyConfig.RootPageTOCName != string.Empty))
					{
						rootPageTOCName = MyConfig.RootPageTOCName;
					}
				}

				htmlHelp = new HtmlHelp(
					workspace.WorkingDirectory,
					MyConfig.HtmlHelpName,
					defaultTopic,
					((MyConfig.OutputTarget & OutputType.HtmlHelp) == 0));

				htmlHelp.IncludeFavorites = MyConfig.IncludeFavorites;
				htmlHelp.BinaryTOC = MyConfig.BinaryTOC;
				htmlHelp.LangID=MyConfig.LangID;

				OnDocBuildingStep(25, "Building file mapping...");

				MakeFilenames();

				string DocLangCode = Enum.GetName(typeof(SdkLanguage),MyConfig.SdkDocLanguage).Replace("_","-");
				utilities = new MsdnXsltUtilities(fileNames, elemNames, MyConfig.SdkDocVersion, DocLangCode, MyConfig.SdkLinksOnWeb, currentFileEncoding);

				OnDocBuildingStep(30, "Loading XSLT files...");

				stylesheets = StyleSheetCollection.LoadStyleSheets(MyConfig.ExtensibilityStylesheet);

				OnDocBuildingStep(40, "Generating HTML pages...");

				htmlHelp.OpenProjectFile();

				htmlHelp.OpenContentsFile(string.Empty, true);

				try
				{
					if (MyConfig.CopyrightHref != null && MyConfig.CopyrightHref != String.Empty)
					{
						if (!MyConfig.CopyrightHref.StartsWith("http:"))
						{
							string copyrightFile = Path.Combine(workspace.WorkingDirectory, Path.GetFileName(MyConfig.CopyrightHref));
							File.Copy(MyConfig.CopyrightHref, copyrightFile, true);
							File.SetAttributes(copyrightFile, FileAttributes.Archive);
							htmlHelp.AddFileToProject(Path.GetFileName(MyConfig.CopyrightHref));
						}
					}

					// add root page if requested
					if (rootPageFileName != null)
					{
						if (!File.Exists(rootPageFileName))
						{
							throw new DocumenterException("Cannot find the documentation's root page file:\n" 
								+ rootPageFileName);
						}

						// add the file
						string rootPageOutputName = Path.Combine(workspace.WorkingDirectory, "default.html");
						if (Path.GetFullPath(rootPageFileName) != Path.GetFullPath(rootPageOutputName))
						{
							File.Copy(rootPageFileName, rootPageOutputName, true);
							File.SetAttributes(rootPageOutputName, FileAttributes.Archive);
						}
						htmlHelp.AddFileToProject(Path.GetFileName(rootPageOutputName));
						htmlHelp.AddFileToContents(rootPageTOCName, 
							Path.GetFileName(rootPageOutputName));

						// depending on peer setting, make root page the container
						if (MyConfig.RootPageContainsNamespaces) htmlHelp.OpenBookInContents();
					}

					documentedNamespaces = new ArrayList();
					MakeHtmlForAssemblies();

					// close root book if applicable
					if (rootPageFileName != null)
					{
						if (MyConfig.RootPageContainsNamespaces) htmlHelp.CloseBookInContents();
					}
				}
				finally
				{
					htmlHelp.CloseContentsFile();
					htmlHelp.CloseProjectFile();
				}

				htmlHelp.WriteEmptyIndexFile();

				if ((MyConfig.OutputTarget & OutputType.Web) > 0)
				{
					OnDocBuildingStep(75, "Generating HTML content file...");

					// Write the embedded online templates to the html output directory
					EmbeddedResources.WriteEmbeddedResources(
						this.GetType().Module.Assembly,
						"NDoc.Documenter.Msdn.onlinefiles",
						workspace.WorkingDirectory);

					using (TemplateWriter indexWriter = new TemplateWriter(
							   Path.Combine(workspace.WorkingDirectory, "index.html"),
							   new StreamReader(this.GetType().Module.Assembly.GetManifestResourceStream(
							   "NDoc.Documenter.Msdn.onlinetemplates.index.html"))))
					{
						indexWriter.CopyToLine("\t\t<title><%TITLE%></title>");
						indexWriter.WriteLine("\t\t<title>" + MyConfig.HtmlHelpName + "</title>");
						indexWriter.CopyToLine("\t\t<frame name=\"main\" src=\"<%HOME_PAGE%>\" frameborder=\"1\">");
						indexWriter.WriteLine("\t\t<frame name=\"main\" src=\"" + defaultTopic + "\" frameborder=\"1\">");
						indexWriter.CopyToEnd();
						indexWriter.Close();
					}

					Trace.WriteLine("transform the HHC contents file into html");
#if DEBUG
					int start = Environment.TickCount;
#endif
					//transform the HHC contents file into html
					using(StreamReader contentsFile = new StreamReader(htmlHelp.GetPathToContentsFile(),Encoding.Default))
					{
						xpathDocument=new XPathDocument(contentsFile);
					}
					using ( StreamWriter streamWriter = new StreamWriter(
								File.Open(Path.Combine(workspace.WorkingDirectory, "contents.html"), FileMode.CreateNew, FileAccess.Write, FileShare.None ), Encoding.Default ) )
					{
#if(NET_1_0)
						//Use overload that is obsolete in v1.1
						stylesheets["htmlcontents"].Transform(xpathDocument, null, streamWriter);
#else
						//Use new overload so we don't get obsolete warnings - clean compile :)
						stylesheets["htmlcontents"].Transform(xpathDocument, null, streamWriter, null);
#endif
					}
#if DEBUG
					Trace.WriteLine((Environment.TickCount - start).ToString() + " msec.");
#endif
				}

				if ((MyConfig.OutputTarget & OutputType.HtmlHelp) > 0)
				{
					OnDocBuildingStep(85, "Compiling HTML Help file...");
					htmlHelp.CompileProject();
				}
				else
				{
#if !DEBUG
					//remove .hhc file
					File.Delete(htmlHelp.GetPathToContentsFile());
#endif
				}

				// if we're only building a CHM, copy that to the Outpur dir
				if ((MyConfig.OutputTarget & OutputType.HtmlHelp) > 0 && (MyConfig.OutputTarget & OutputType.Web) == 0) 
				{
					workspace.SaveOutputs( "*.chm" );
				} 
				else 
				{
					// otherwise copy everything to the output dir (cause the help file is all the html, not just one chm)
					workspace.SaveOutputs( "*.*" );
				}
				
				if ( MyConfig.CleanIntermediates )
					workspace.CleanIntermediates();
				
				OnDocBuildingStep(100, "Done.");
			}
			catch(Exception ex)
			{
				throw new DocumenterException(ex.Message, ex);
			}
			finally
			{
				xmlDocumentation = null;
 				xpathDocument = null;
 				stylesheets = null;
				workspace.RemoveResourceDirectory();
			}
		}

		private MsdnDocumenterConfig MyConfig
		{
			get
			{
				return (MsdnDocumenterConfig)Config;
			}
		}

		private void MakeFilenames()
		{
			XmlNodeList namespaces = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace");
			foreach (XmlElement namespaceNode in namespaces)
			{
				string namespaceName = namespaceNode.Attributes["name"].Value;
				string namespaceId = "N:" + namespaceName;
				fileNames[namespaceId] = GetFilenameForNamespace(namespaceName);
				elemNames[namespaceId] = namespaceName;

				XmlNodeList types = namespaceNode.SelectNodes("*[@id]");
				foreach (XmlElement typeNode in types)
				{
					string typeId = typeNode.Attributes["id"].Value;
					fileNames[typeId] = GetFilenameForType(typeNode);
					elemNames[typeId] = typeNode.Attributes["name"].Value;

					XmlNodeList members = typeNode.SelectNodes("*[@id]");
					foreach (XmlElement memberNode in members)
					{
						string id = memberNode.Attributes["id"].Value;
						switch (memberNode.Name)
						{
							case "constructor":
								fileNames[id] = GetFilenameForConstructor(memberNode);
								elemNames[id] = elemNames[typeId];
								break;
							case "field":
								if (typeNode.Name == "enumeration")
									fileNames[id] = GetFilenameForType(typeNode);
								else
									fileNames[id] = GetFilenameForField(memberNode);
								elemNames[id] = memberNode.Attributes["name"].Value;
								break;
							case "property":
								fileNames[id] = GetFilenameForProperty(memberNode);
								elemNames[id] = memberNode.Attributes["name"].Value;
								break;
							case "method":
								fileNames[id] = GetFilenameForMethod(memberNode);
								elemNames[id] = memberNode.Attributes["name"].Value;
								break;
							case "operator":
								fileNames[id] = GetFilenameForOperator(memberNode);
								elemNames[id] = memberNode.Attributes["name"].Value;
								break;
							case "event":
								fileNames[id] = GetFilenameForEvent(memberNode);
								elemNames[id] = memberNode.Attributes["name"].Value;
								break;
						}
					}
				}
			}
		}


		private WhichType GetWhichType(XmlNode typeNode)
		{
			WhichType whichType;

			switch (typeNode.Name)
			{
				case "class":
					whichType = WhichType.Class;
					break;
				case "interface":
					whichType = WhichType.Interface;
					break;
				case "structure":
					whichType = WhichType.Structure;
					break;
				case "enumeration":
					whichType = WhichType.Enumeration;
					break;
				case "delegate":
					whichType = WhichType.Delegate;
					break;
				default:
					whichType = WhichType.Unknown;
					break;
			}

			return whichType;
		}

		private void MakeHtmlForAssemblies()
		{
#if DEBUG
			int start = Environment.TickCount;
#endif

			MakeHtmlForAssembliesSorted();

#if DEBUG
			Trace.WriteLine("Making Html: " + ((Environment.TickCount - start)/1000.0).ToString() + " sec.");
#endif
		}

		private void MakeHtmlForAssembliesSorted()
		{
			XmlNodeList assemblyNodes = xmlDocumentation.SelectNodes("/ndoc/assembly");
			bool        heirTOC = (this.MyConfig.NamespaceTOCStyle == TOCStyle.Hierarchical);
			int         level = 0;
			int[] indexes = SortNodesByAttribute(assemblyNodes, "name");
			string[]    last = new string[0];

			System.Collections.Specialized.NameValueCollection namespaceAssemblies
				= new System.Collections.Specialized.NameValueCollection();

			int nNodes = assemblyNodes.Count;
			for (int i = 0; i < nNodes; i++)
			{
				XmlNode assemblyNode = assemblyNodes[indexes[i]];
				if (assemblyNode.ChildNodes.Count > 0)
				{
					string assemblyName = (string)assemblyNode.Attributes["name"].Value;
					GetNamespacesFromAssembly(assemblyName, namespaceAssemblies);
				}
			}

			string [] namespaces = namespaceAssemblies.AllKeys;
			Array.Sort(namespaces);
			nNodes = namespaces.Length;

			for (int i = 0; i < nNodes; i++)
			{
				OnDocBuildingProgress(i*100/nNodes);
				
				if (heirTOC) 
				{
					string[] split = namespaces[i].Split('.');

					for (level = last.Length; level >= 0 &&
						ArrayEquals(split, 0, last, 0, level) == false; level--)
					{
						if (level > last.Length) 
							continue;
						
						MakeHtmlForTypes(string.Join(".", last, 0, level));
						htmlHelp.CloseBookInContents();
					}
	        
					if (level < 0) level = 0;

					for (; level < split.Length; level++)
					{
						string ns = string.Join(".", split, 0, level + 1);
						
						if (Array.BinarySearch(namespaces, ns) < 0)
							MakeHtmlForNamespace(split[level], ns, false);
						else
							MakeHtmlForNamespace(split[level], ns, true);

						htmlHelp.OpenBookInContents();
					}

					last = split;
				}
				else
				{
					MakeHtmlForNamespace(namespaces[i], namespaces[i], true);
					htmlHelp.OpenBookInContents();
					MakeHtmlForTypes(namespaces[i]);
					htmlHelp.CloseBookInContents();
				}
			}

			
			if (heirTOC && last.Length > 0)
			{
				for (; level >= 1; level--)
				{
					MakeHtmlForTypes(string.Join(".", last, 0, level));
					htmlHelp.CloseBookInContents();
				}
			}

			OnDocBuildingProgress(100);
		}

		private bool ArrayEquals(string[] array1, int from1, string[] array2, int from2, int count)
		{
			for (int i = 0; i < count; i++)
			{
				if (array1[from1 + i] != array2[from2 + i])
					return false;
			}

			return true;
		}

		private void GetNamespacesFromAssembly(string assemblyName, System.Collections.Specialized.NameValueCollection namespaceAssemblies)
		{
			XmlNodeList namespaceNodes = xmlDocumentation.SelectNodes("/ndoc/assembly[@name=\"" + assemblyName + "\"]/module/namespace");
			foreach (XmlNode namespaceNode in namespaceNodes)
			{
				string namespaceName = (string)namespaceNode.Attributes["name"].Value;
				namespaceAssemblies.Add(namespaceName, assemblyName);
			}
		}

		/// <summary>
		/// Add the namespace elements to the output
		/// </summary>
		/// <remarks>
		/// The namespace 
		/// </remarks>
		/// <param name="namespacePart">If nested, the namespace part will be the current
		/// namespace element being documented</param>
		/// <param name="namespaceName">The full namespace name being documented</param>
		/// <param name="addDocumentation">If true, the namespace will be documented, if false
		/// the node in the TOC will not link to a page</param>
		private void MakeHtmlForNamespace(string namespacePart, string namespaceName, 
			bool addDocumentation)
		{
			if (documentedNamespaces.Contains(namespaceName)) 
				return;

			documentedNamespaces.Add(namespaceName);

			if (addDocumentation)
			{
			string fileName = GetFilenameForNamespace(namespaceName);
				
				htmlHelp.AddFileToContents(namespacePart, fileName);

			XsltArgumentList arguments = new XsltArgumentList();
			arguments.AddParam("namespace", String.Empty, namespaceName);

			TransformAndWriteResult("namespace", arguments, fileName);

			arguments = new XsltArgumentList();
			arguments.AddParam("namespace", String.Empty, namespaceName);

			TransformAndWriteResult(
				"namespacehierarchy",
				arguments,
				fileName.Insert(fileName.Length - 5, "Hierarchy"));
			}
			else
				htmlHelp.AddFileToContents(namespacePart);
		}

		private void MakeHtmlForTypes(string namespaceName)
		{
			XmlNodeList typeNodes =
				xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace[@name=\"" + namespaceName + "\"]/*[local-name()!='documentation' and local-name()!='typeHierarchy']");

			int[] indexes = SortNodesByAttribute(typeNodes, "id");
			int nNodes = typeNodes.Count;

			for (int i = 0; i < nNodes; i++)
			{
				XmlNode typeNode = typeNodes[indexes[i]];

				WhichType whichType = GetWhichType(typeNode);

				switch(whichType)
				{
					case WhichType.Class:
						MakeHtmlForInterfaceOrClassOrStructure(whichType, typeNode);
						break;
					case WhichType.Interface:
						MakeHtmlForInterfaceOrClassOrStructure(whichType, typeNode);
						break;
					case WhichType.Structure:
						MakeHtmlForInterfaceOrClassOrStructure(whichType, typeNode);
						break;
					case WhichType.Enumeration:
						MakeHtmlForEnumerationOrDelegate(whichType, typeNode);
						break;
					case WhichType.Delegate:
						MakeHtmlForEnumerationOrDelegate(whichType, typeNode);
						break;
					default:
						break;
				}
			}
		}

		private void MakeHtmlForEnumerationOrDelegate(WhichType whichType, XmlNode typeNode)
		{
			string typeName = typeNode.Attributes["name"].Value;
			string typeID = typeNode.Attributes["id"].Value;
			string fileName = GetFilenameForType(typeNode);

			htmlHelp.AddFileToContents(typeName + " " + mixedCaseTypeNames[whichType], fileName, HtmlHelpIcon.Page );

			XsltArgumentList arguments = new XsltArgumentList();
			arguments.AddParam("type-id", String.Empty, typeID);
			TransformAndWriteResult("type", arguments, fileName);
		}

		private void MakeHtmlForInterfaceOrClassOrStructure(
			WhichType whichType,
			XmlNode typeNode)
		{
			string typeName = typeNode.Attributes["name"].Value;
			string typeID = typeNode.Attributes["id"].Value;
			string fileName = GetFilenameForType(typeNode);

			htmlHelp.AddFileToContents(typeName + " " + mixedCaseTypeNames[whichType], fileName);

			bool hasMembers = typeNode.SelectNodes("constructor|field|property|method|operator|event").Count > 0;

			if (hasMembers)
			{
				htmlHelp.OpenBookInContents();
			}

			XsltArgumentList arguments = new XsltArgumentList();
			arguments.AddParam("type-id", String.Empty, typeID);
			TransformAndWriteResult("type", arguments, fileName);

			if ( typeNode.SelectNodes( "derivedBy" ).Count > 5 )
			{
				fileName = GetFilenameForTypeHierarchy(typeNode);
				arguments = new XsltArgumentList();
				arguments.AddParam("type-id", String.Empty, typeID);
				TransformAndWriteResult("typehierarchy", arguments, fileName);
			}

			if (hasMembers)
			{
				fileName = GetFilenameForTypeMembers(typeNode);
				htmlHelp.AddFileToContents(typeName + " Members", 
					fileName, 
					HtmlHelpIcon.Page);

				arguments = new XsltArgumentList();
				arguments.AddParam("id", String.Empty, typeID);
				TransformAndWriteResult("allmembers", arguments, fileName);

				MakeHtmlForConstructors(whichType, typeNode);
				MakeHtmlForFields(whichType, typeNode);
				MakeHtmlForProperties(whichType, typeNode);
				MakeHtmlForMethods(whichType, typeNode);
				MakeHtmlForOperators(whichType, typeNode);
				MakeHtmlForEvents(whichType, typeNode);

				htmlHelp.CloseBookInContents();
			}
		}

		private void MakeHtmlForConstructors(WhichType whichType, XmlNode typeNode)
		{
			XmlNodeList   constructorNodes;
			string        constructorID;
			string        typeName;
			//string        typeID;
			string        fileName;

			typeName = typeNode.Attributes["name"].Value;
			//typeID = typeNode.Attributes["id"].Value;
			constructorNodes = typeNode.SelectNodes("constructor[@contract!='Static']");

			// If the constructor is overloaded then make an overload page.
			if (constructorNodes.Count > 1)
			{
				fileName = GetFilenameForConstructors(typeNode);
				htmlHelp.AddFileToContents(typeName + " Constructor", fileName);

				htmlHelp.OpenBookInContents();

				constructorID = constructorNodes[0].Attributes["id"].Value;

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("member-id", String.Empty, constructorID);
				TransformAndWriteResult("memberoverload", arguments, fileName);
			}

			foreach (XmlNode constructorNode in constructorNodes)
			{
				constructorID = constructorNode.Attributes["id"].Value;
				fileName = GetFilenameForConstructor(constructorNode);

				if (constructorNodes.Count > 1)
				{
					XmlNodeList   parameterNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace/" + lowerCaseTypeNames[whichType] + "[@name=\"" + typeName + "\"]/constructor[@id=\"" + constructorID + "\"]/parameter");
					htmlHelp.AddFileToContents(typeName + " Constructor " + GetParamList(parameterNodes), fileName,
						HtmlHelpIcon.Page );
				}
				else
				{
					htmlHelp.AddFileToContents(typeName + " Constructor", fileName, HtmlHelpIcon.Page );
				}

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("member-id", String.Empty, constructorID);
				TransformAndWriteResult("member", arguments, fileName);
			}

			if (constructorNodes.Count > 1)
			{
				htmlHelp.CloseBookInContents();
			}

			XmlNode staticConstructorNode = typeNode.SelectSingleNode("constructor[@contract='Static']");
			if (staticConstructorNode != null)
			{
				constructorID = staticConstructorNode.Attributes["id"].Value;
				fileName = GetFilenameForConstructor(staticConstructorNode);

				htmlHelp.AddFileToContents(typeName + " Static Constructor", fileName, HtmlHelpIcon.Page);

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("member-id", String.Empty, constructorID);
				TransformAndWriteResult("member", arguments, fileName);
			}
		}

		private void MakeHtmlForFields(WhichType whichType, XmlNode typeNode)
		{
			XmlNodeList fields = typeNode.SelectNodes("field[not(@declaringType)]");

			if (fields.Count > 0)
			{
				//string typeName = typeNode.Attributes["name"].Value;
				string typeID = typeNode.Attributes["id"].Value;
				string fileName = GetFilenameForFields(whichType, typeNode);

				htmlHelp.AddFileToContents("Fields", fileName);

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("id", String.Empty, typeID);
				arguments.AddParam("member-type", String.Empty, "field");
				TransformAndWriteResult("individualmembers", arguments, fileName);

				htmlHelp.OpenBookInContents();

				int[] indexes = SortNodesByAttribute(fields, "id");

				foreach (int index in indexes)
				{
					XmlNode field = fields[index];

					string fieldName = field.Attributes["name"].Value;
					string fieldID = field.Attributes["id"].Value;
					fileName = GetFilenameForField(field);
					htmlHelp.AddFileToContents(fieldName + " Field", fileName, HtmlHelpIcon.Page );

					arguments = new XsltArgumentList();
					arguments.AddParam("field-id", String.Empty, fieldID);
					TransformAndWriteResult("field", arguments, fileName);
				}

				htmlHelp.CloseBookInContents();
			}
		}

		private void MakeHtmlForProperties(WhichType whichType, XmlNode typeNode)
		{
			XmlNodeList declaredPropertyNodes = typeNode.SelectNodes("property[not(@declaringType)]");

			if (declaredPropertyNodes.Count > 0)
			{
				XmlNodeList   propertyNodes;
				XmlNode     propertyNode;
				string        propertyName;
				string        propertyID;
				string        previousPropertyName;
				string        nextPropertyName;
				bool        bOverloaded = false;
				string        typeName;
				string        typeID;
				string        fileName;
				int         nNodes;
				int[]       indexes;
				int         i;

				typeName = typeNode.Attributes["name"].Value;
				typeID = typeNode.Attributes["id"].Value;
				propertyNodes = typeNode.SelectNodes("property[not(@declaringType)]");
				nNodes = propertyNodes.Count;

				indexes = SortNodesByAttribute(propertyNodes, "id");

				fileName = GetFilenameForProperties(whichType, typeNode);
				htmlHelp.AddFileToContents("Properties", fileName);

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("id", String.Empty, typeID);
				arguments.AddParam("member-type", String.Empty, "property");
				TransformAndWriteResult("individualmembers", arguments, fileName);

				htmlHelp.OpenBookInContents();

				for (i = 0; i < nNodes; i++)
				{
					propertyNode = propertyNodes[indexes[i]];

					propertyName = (string)propertyNode.Attributes["name"].Value;
					propertyID = (string)propertyNode.Attributes["id"].Value;

					// If the method is overloaded then make an overload page.
					previousPropertyName = ((i - 1 < 0) || (propertyNodes[indexes[i - 1]].Attributes.Count == 0))
						? "" : propertyNodes[indexes[i - 1]].Attributes[0].Value;
					nextPropertyName = ((i + 1 == nNodes) || (propertyNodes[indexes[i + 1]].Attributes.Count == 0))
						? "" : propertyNodes[indexes[i + 1]].Attributes[0].Value;

					if ((previousPropertyName != propertyName) && (nextPropertyName == propertyName))
					{
						fileName = GetFilenameForPropertyOverloads(typeNode, propertyNode);
						htmlHelp.AddFileToContents(propertyName + " Property", fileName);

						arguments = new XsltArgumentList();
						arguments.AddParam("member-id", String.Empty, propertyID);
						TransformAndWriteResult("memberoverload", arguments, fileName);

						htmlHelp.OpenBookInContents();

						bOverloaded = true;
					}

					fileName = GetFilenameForProperty(propertyNode);

					if (bOverloaded)
					{
						XmlNodeList parameterNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace/" + lowerCaseTypeNames[whichType] + "[@name=\"" + typeName + "\"]/property[@id=\"" + propertyID + "\"]/parameter");
						htmlHelp.AddFileToContents(propertyName + " Property " + GetParamList(parameterNodes), fileName,
							HtmlHelpIcon.Page );
					}
					else
					{
						htmlHelp.AddFileToContents(propertyName + " Property", fileName, 
							HtmlHelpIcon.Page );
					}

					XsltArgumentList arguments2 = new XsltArgumentList();
					arguments2.AddParam("property-id", String.Empty, propertyID);
					TransformAndWriteResult("property", arguments2, fileName);

					if ((previousPropertyName == propertyName) && (nextPropertyName != propertyName))
					{
						htmlHelp.CloseBookInContents();
						bOverloaded = false;
					}
				}

				htmlHelp.CloseBookInContents();
			}
		}

		private string GetPreviousMethodName(XmlNodeList methodNodes, int[] indexes, int index)
		{
			while (--index >= 0)
			{
				if (methodNodes[indexes[index]].Attributes["declaringType"] == null)
					return methodNodes[indexes[index]].Attributes["name"].Value;
			}
			return null;
		}

		private string GetNextMethodName(XmlNodeList methodNodes, int[] indexes, int index)
		{
			while (++index < methodNodes.Count)
			{
				if (methodNodes[indexes[index]].Attributes["declaringType"] == null)
					return methodNodes[indexes[index]].Attributes["name"].Value;
			}
			return null;
		}

		// returns true, if method is neither overload of a method in the same class,
		// nor overload of a method in the base class.
		private bool IsMethodAlone(XmlNodeList methodNodes, int[] indexes, int index)
		{
			string name = methodNodes[indexes[index]].Attributes["name"].Value;
			int lastIndex = methodNodes.Count - 1;
			if (lastIndex <= 0)
				return true;
			bool previousNameDifferent = (index == 0)
				|| (methodNodes[indexes[index - 1]].Attributes["name"].Value != name);
			bool nextNameDifferent = (index == lastIndex)
				|| (methodNodes[indexes[index + 1]].Attributes["name"].Value != name);
			return (previousNameDifferent && nextNameDifferent);
		}

		private bool IsMethodFirstOverload(XmlNodeList methodNodes, int[] indexes, int index)
		{
			if ((methodNodes[indexes[index]].Attributes["declaringType"] != null)
				|| IsMethodAlone(methodNodes, indexes, index))
				return false;

			string name			= methodNodes[indexes[index]].Attributes["name"].Value;
			string previousName	= GetPreviousMethodName(methodNodes, indexes, index);
			return previousName != name;
		}

		private bool IsMethodLastOverload(XmlNodeList methodNodes, int[] indexes, int index)
		{
			if ((methodNodes[indexes[index]].Attributes["declaringType"] != null)
				|| IsMethodAlone(methodNodes, indexes, index))
				return false;

			string name		= methodNodes[indexes[index]].Attributes["name"].Value;
			string nextName	= GetNextMethodName(methodNodes, indexes, index);
			return nextName != name;
		}

		private void MakeHtmlForMethods(WhichType whichType, XmlNode typeNode)
		{
			XmlNodeList declaredMethodNodes = typeNode.SelectNodes("method[not(@declaringType)]");

			if (declaredMethodNodes.Count > 0)
			{
				bool bOverloaded = false;
				string fileName;

				string typeName = typeNode.Attributes["name"].Value;
				string typeID = typeNode.Attributes["id"].Value;
				XmlNodeList methodNodes = typeNode.SelectNodes("method");
				int nNodes = methodNodes.Count;

				int[] indexes = SortNodesByAttribute(methodNodes, "id");

				fileName = GetFilenameForMethods(whichType, typeNode);
				htmlHelp.AddFileToContents("Methods", fileName);

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("id", String.Empty, typeID);
				arguments.AddParam("member-type", String.Empty, "method");
				TransformAndWriteResult("individualmembers", arguments, fileName);

				htmlHelp.OpenBookInContents();

				for (int i = 0; i < nNodes; i++)
				{
					XmlNode methodNode = methodNodes[indexes[i]];
					string methodName = (string)methodNode.Attributes["name"].Value;
					string methodID = (string)methodNode.Attributes["id"].Value;

					if (IsMethodFirstOverload(methodNodes, indexes, i))
					{
						bOverloaded = true;

						fileName = GetFilenameForMethodOverloads(typeNode, methodNode);
						htmlHelp.AddFileToContents(methodName + " Method", fileName);

						arguments = new XsltArgumentList();
						arguments.AddParam("member-id", String.Empty, methodID);
						TransformAndWriteResult("memberoverload", arguments, fileName);

						htmlHelp.OpenBookInContents();
					}

					if (methodNode.Attributes["declaringType"] == null)
					{
						fileName = GetFilenameForMethod(methodNode);

						if (bOverloaded)
						{
							XmlNodeList parameterNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace/" + lowerCaseTypeNames[whichType] + "[@name=\"" + typeName + "\"]/method[@id=\"" + methodID + "\"]/parameter");
							htmlHelp.AddFileToContents(methodName + " Method " + GetParamList(parameterNodes), fileName,
								HtmlHelpIcon.Page );
						}
						else
						{
							htmlHelp.AddFileToContents(methodName + " Method", fileName,
								HtmlHelpIcon.Page );
						}

						XsltArgumentList arguments2 = new XsltArgumentList();
						arguments2.AddParam("member-id", String.Empty, methodID);
						TransformAndWriteResult("member", arguments2, fileName);
					}

					if (bOverloaded && IsMethodLastOverload(methodNodes, indexes, i))
					{
						bOverloaded = false;
						htmlHelp.CloseBookInContents();
					}
				}

				htmlHelp.CloseBookInContents();
			}
		}

		private void MakeHtmlForOperators(WhichType whichType, XmlNode typeNode)
		{
			XmlNodeList operators = typeNode.SelectNodes("operator");

			if (operators.Count > 0)
			{
				string typeName = (string)typeNode.Attributes["name"].Value;
				string typeID = (string)typeNode.Attributes["id"].Value;
				XmlNodeList opNodes = typeNode.SelectNodes("operator");
				string fileName = GetFilenameForOperators(whichType, typeNode);
				bool bOverloaded = false;

				bool bHasOperators =  (typeNode.SelectSingleNode("operator[@name != 'op_Explicit' and @name != 'op_Implicit']") != null);;
				bool bHasConverters = (typeNode.SelectSingleNode("operator[@name  = 'op_Explicit' or  @name  = 'op_Implicit']") != null);
				string title="";

				if (bHasOperators)
				{
					if (bHasConverters)
					{
						title = "Operators and Type Conversions";
					}
					else
					{
						title = "Operators";
					}
				}
				else
				{
					if (bHasConverters)
					{
						title = "Type Conversions";
					}
				}

				htmlHelp.AddFileToContents(title, fileName);

				XsltArgumentList arguments = new XsltArgumentList();
				arguments.AddParam("id", String.Empty, typeID);
				arguments.AddParam("member-type", String.Empty, "operator");
				TransformAndWriteResult("individualmembers", arguments, fileName);

				htmlHelp.OpenBookInContents();

				int[] indexes = SortNodesByAttribute(operators, "id");
				int nNodes = opNodes.Count;

				//operators first
				for (int i = 0; i < nNodes; i++)
				{
					XmlNode operatorNode = operators[indexes[i]];
					string operatorID = operatorNode.Attributes["id"].Value;

					string opName = (string)operatorNode.Attributes["name"].Value;
					if ((opName != "op_Implicit") && (opName != "op_Explicit"))
					{
						if (IsMethodFirstOverload(opNodes, indexes, i))
						{
							bOverloaded = true;

							fileName = GetFilenameForOperatorsOverloads(typeNode, operatorNode);
							htmlHelp.AddFileToContents(GetOperatorName(operatorNode), fileName);

							arguments = new XsltArgumentList();
							arguments.AddParam("member-id", String.Empty, operatorID);
							TransformAndWriteResult("memberoverload", arguments, fileName);

							htmlHelp.OpenBookInContents();
						}


						fileName = GetFilenameForOperator(operatorNode);
						if (bOverloaded)
						{
							XmlNodeList parameterNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace/" + lowerCaseTypeNames[whichType] + "[@name=\"" + typeName + "\"]/operator[@id=\"" + operatorID + "\"]/parameter");
							htmlHelp.AddFileToContents(GetOperatorName(operatorNode) + GetParamList(parameterNodes), fileName, 
								HtmlHelpIcon.Page);
						}
						else
						{
							htmlHelp.AddFileToContents(GetOperatorName(operatorNode), fileName, 
								HtmlHelpIcon.Page );
						}

						arguments = new XsltArgumentList();
						arguments.AddParam("member-id", String.Empty, operatorID);
						TransformAndWriteResult("member", arguments, fileName);

						if (bOverloaded && IsMethodLastOverload(opNodes, indexes, i))
						{
							bOverloaded = false;
							htmlHelp.CloseBookInContents();
						}
					}
				}

				//type converters
				for (int i = 0; i < nNodes; i++)
				{
					XmlNode operatorNode = operators[indexes[i]];
					string operatorID = operatorNode.Attributes["id"].Value;

					string opName = (string)operatorNode.Attributes["name"].Value;
					if ((opName == "op_Implicit") || (opName == "op_Explicit"))
					{
						fileName = GetFilenameForOperator(operatorNode);
						htmlHelp.AddFileToContents(GetOperatorName(operatorNode), fileName, 
							HtmlHelpIcon.Page );

						arguments = new XsltArgumentList();
						arguments.AddParam("member-id", String.Empty, operatorID);
						TransformAndWriteResult("member", arguments, fileName);

					}
				}

				htmlHelp.CloseBookInContents();
			}
		}

		private string GetOperatorName(XmlNode operatorNode)
		{
			string name = operatorNode.Attributes["name"].Value;

			switch (name)
			{
				case "op_Decrement": return "Decrement Operator";
				case "op_Increment": return "Increment Operator";
				case "op_UnaryNegation": return "Unary Negation Operator";
				case "op_UnaryPlus": return "Unary Plus Operator";
				case "op_LogicalNot": return "Logical Not Operator";
				case "op_True": return "True Operator";
				case "op_False": return "False Operator";
				case "op_AddressOf": return "Address Of Operator";
				case "op_OnesComplement": return "Ones Complement Operator";
				case "op_PointerDereference": return "Pointer Dereference Operator";
				case "op_Addition": return "Addition Operator";
				case "op_Subtraction": return "Subtraction Operator";
				case "op_Multiply": return "Multiplication Operator";
				case "op_Division": return "Division Operator";
				case "op_Modulus": return "Modulus Operator";
				case "op_ExclusiveOr": return "Exclusive Or Operator";
				case "op_BitwiseAnd": return "Bitwise And Operator";
				case "op_BitwiseOr": return "Bitwise Or Operator";
				case "op_LogicalAnd": return "LogicalAnd Operator";
				case "op_LogicalOr": return "Logical Or Operator";
				case "op_Assign": return "Assignment Operator";
				case "op_LeftShift": return "Left Shift Operator";
				case "op_RightShift": return "Right Shift Operator";
				case "op_SignedRightShift": return "Signed Right Shift Operator";
				case "op_UnsignedRightShift": return "Unsigned Right Shift Operator";
				case "op_Equality": return "Equality Operator";
				case "op_GreaterThan": return "Greater Than Operator";
				case "op_LessThan": return "Less Than Operator";
				case "op_Inequality": return "Inequality Operator";
				case "op_GreaterThanOrEqual": return "Greater Than Or Equal Operator";
				case "op_LessThanOrEqual": return "Less Than Or Equal Operator";
				case "op_UnsignedRightShiftAssignment": return "Unsigned Right Shift Assignment Operator";
				case "op_MemberSelection": return "Member Selection Operator";
				case "op_RightShiftAssignment": return "Right Shift Assignment Operator";
				case "op_MultiplicationAssignment": return "Multiplication Assignment Operator";
				case "op_PointerToMemberSelection": return "Pointer To Member Selection Operator";
				case "op_SubtractionAssignment": return "Subtraction Assignment Operator";
				case "op_ExclusiveOrAssignment": return "Exclusive Or Assignment Operator";
				case "op_LeftShiftAssignment": return "Left Shift Assignment Operator";
				case "op_ModulusAssignment": return "Modulus Assignment Operator";
				case "op_AdditionAssignment": return "Addition Assignment Operator";
				case "op_BitwiseAndAssignment": return "Bitwise And Assignment Operator";
				case "op_BitwiseOrAssignment": return "Bitwise Or Assignment Operator";
				case "op_Comma": return "Comma Operator";
				case "op_DivisionAssignment": return "Division Assignment Operator";
				case "op_Explicit":
					XmlNode parameterNode = operatorNode.SelectSingleNode("parameter");
					string from = parameterNode.Attributes["type"].Value;
					string to = operatorNode.Attributes["returnType"].Value;
					return "Explicit " + StripNamespace(from) + " to " + StripNamespace(to) + " Conversion";
				case "op_Implicit":
					XmlNode parameterNode2 = operatorNode.SelectSingleNode("parameter");
					string from2 = parameterNode2.Attributes["type"].Value;
					string to2 = operatorNode.Attributes["returnType"].Value;
					return "Implicit " + StripNamespace(from2) + " to " + StripNamespace(to2) + " Conversion";
				default:
					return "ERROR";
			}
		}

		private string StripNamespace(string name)
		{
			string result = name;

			int lastDot = name.LastIndexOf('.');

			if (lastDot != -1)
			{
				result = name.Substring(lastDot + 1);
			}

			return result;
		}

		private void MakeHtmlForEvents(WhichType whichType, XmlNode typeNode)
		{
			XmlNodeList declaredEventNodes = typeNode.SelectNodes("event[not(@declaringType)]");

			if (declaredEventNodes.Count > 0)
			{
				XmlNodeList events = typeNode.SelectNodes("event");

				if (events.Count > 0)
				{
					//string typeName = (string)typeNode.Attributes["name"].Value;
					string typeID = (string)typeNode.Attributes["id"].Value;
					string fileName = GetFilenameForEvents(whichType, typeNode);

					htmlHelp.AddFileToContents("Events", fileName);

					XsltArgumentList arguments = new XsltArgumentList();
					arguments.AddParam("id", String.Empty, typeID);
					arguments.AddParam("member-type", String.Empty, "event");
					TransformAndWriteResult("individualmembers", arguments, fileName);

					htmlHelp.OpenBookInContents();

					int[] indexes = SortNodesByAttribute(events, "id");

					foreach (int index in indexes)
					{
						XmlNode eventElement = events[index];

						if (eventElement.Attributes["declaringType"] == null)
						{
							string eventName = (string)eventElement.Attributes["name"].Value;
							string eventID = (string)eventElement.Attributes["id"].Value;

							fileName = GetFilenameForEvent(eventElement);
							htmlHelp.AddFileToContents(eventName + " Event", 
								fileName, 
								HtmlHelpIcon.Page);

							arguments = new XsltArgumentList();
							arguments.AddParam("event-id", String.Empty, eventID);
							TransformAndWriteResult("event", arguments, fileName);
						}
					}

					htmlHelp.CloseBookInContents();
				}
			}
		}

		private string GetParamList(XmlNodeList parameterNodes)
		{
			int numberOfNodes = parameterNodes.Count;
			int nodeIndex = 1;
			string paramList = "(";

			foreach (XmlNode parameterNode in parameterNodes)
			{
				paramList += StripNamespace(parameterNode.Attributes["type"].Value);

				if (nodeIndex < numberOfNodes)
				{
					paramList += ", ";
				}

				nodeIndex++;
			}

			paramList += ")";

			return paramList;
		}

		private int[] SortNodesByAttribute(XmlNodeList nodes, string attributeName)
		{
			int length = nodes.Count;
			string[] names = new string[length];
			int[] indexes = new int[length];
			int i = 0;

			foreach (XmlNode node in nodes)
			{
				names[i] = (string)node.Attributes[attributeName].Value;
				indexes[i] = i++;
			}

			Array.Sort(names, indexes);

			return indexes;
		}

		private void TransformAndWriteResult(
			string transformName,
			XsltArgumentList arguments,
			string filename)
		{
			Trace.WriteLine(filename);
#if DEBUG
			int start = Environment.TickCount;
#endif

			ExternalHtmlProvider htmlProvider = new ExternalHtmlProvider(MyConfig, filename);
			StreamWriter streamWriter = null;

			try
			{
				using (streamWriter =  new StreamWriter(
					File.Open(Path.Combine(workspace.WorkingDirectory, filename), FileMode.Create),
					currentFileEncoding))
				{
					arguments.AddParam("ndoc-title", String.Empty, MyConfig.Title);
					arguments.AddParam("ndoc-vb-syntax", String.Empty, MyConfig.ShowVisualBasic);
					arguments.AddParam("ndoc-omit-object-tags", String.Empty, ((MyConfig.OutputTarget & OutputType.HtmlHelp) == 0));
					arguments.AddParam("ndoc-document-attributes", String.Empty, MyConfig.DocumentAttributes);
					arguments.AddParam("ndoc-documented-attributes", String.Empty, MyConfig.DocumentedAttributes);

					arguments.AddParam("ndoc-sdk-doc-base-url", String.Empty, utilities.SdkDocBaseUrl);
					arguments.AddParam("ndoc-sdk-doc-file-ext", String.Empty, utilities.SdkDocExt);

					arguments.AddExtensionObject("urn:NDocUtil", utilities);
					arguments.AddExtensionObject("urn:NDocExternalHtml", htmlProvider);

					//reset overloads testing
					utilities.Reset();

					XslTransform transform = stylesheets[transformName];

#if (NET_1_0)
				//Use overload that is now obsolete
				transform.Transform(xpathDocument, arguments, streamWriter);
#else           
					//Use new overload so we don't get obsolete warnings - clean compile :)
					transform.Transform(xpathDocument, arguments, streamWriter, null);
#endif
				}
			}
			catch(PathTooLongException e)
			{
				throw new PathTooLongException(e.Message + "\nThe file that NDoc was trying to create had the following name:\n" + Path.Combine(workspace.WorkingDirectory, filename));
			}

#if DEBUG
			Debug.WriteLine((Environment.TickCount - start).ToString() + " msec.");
#endif
			htmlHelp.AddFileToProject(filename);
		}

		private string GetFilenameForNamespace(string namespaceName)
		{
			string fileName = namespaceName + ".html";
			return fileName;
		}

		private string GetFilenameForType(XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + ".html";
			return fileName;
		}

		private string GetFilenameForTypeHierarchy(XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Hierarchy.html";
			return fileName;
		}
		private string GetFilenameForTypeMembers(XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Members.html";
			return fileName;
		}

		private string GetFilenameForConstructors(XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Constructor.html";
			return fileName;
		}

		private string GetFilenameForConstructor(XmlNode constructorNode)
		{
			string constructorID = (string)constructorNode.Attributes["id"].Value;
			int dotHash = constructorID.IndexOf(".#"); // constructors could be #ctor or #cctor

			string fileName = constructorID.Substring(2, dotHash - 2);
			if (constructorNode.Attributes["contract"].Value == "Static")
				fileName += "Static";

			fileName += "Constructor";

			if (constructorNode.Attributes["overload"] != null)
			{
				fileName += (string)constructorNode.Attributes["overload"].Value;
			}

			fileName += ".html";

			return fileName;
		}

		private string GetFilenameForFields(WhichType whichType, XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Fields.html";
			return fileName;
		}

		private string GetFilenameForField(XmlNode fieldNode)
		{
			string fieldID = (string)fieldNode.Attributes["id"].Value;
			string fileName = fieldID.Substring(2) + ".html";
			fileName = fileName.Replace("#",".");
			return fileName;
		}

		private string GetFilenameForOperators(WhichType whichType, XmlNode typeNode)
		{
			string typeID = typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Operators.html";
			return fileName;
		}

		private string GetFilenameForOperatorsOverloads(XmlNode typeNode, XmlNode opNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string opName = (string)opNode.Attributes["name"].Value;
			string fileName = typeID.Substring(2) + "." + opName + "_overloads.html";
			return fileName;
		}

		private string GetFilenameForOperator(XmlNode operatorNode)
		{
			string operatorID = operatorNode.Attributes["id"].Value;
			string fileName = operatorID.Substring(2);

			int leftParenIndex = fileName.IndexOf('(');

			if (leftParenIndex != -1)
			{
				fileName = fileName.Substring(0, leftParenIndex);
			}

			if (operatorNode.Attributes["overload"] != null)
			{
				fileName += "_overload_" + operatorNode.Attributes["overload"].Value;
			}

			fileName += ".html";
			fileName = fileName.Replace("#",".");

			return fileName;
		}

		private string GetFilenameForEvents(WhichType whichType, XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Events.html";
			return fileName;
		}

		private string GetFilenameForEvent(XmlNode eventNode)
		{
			string eventID = (string)eventNode.Attributes["id"].Value;
			string fileName = eventID.Substring(2) + ".html";
			fileName = fileName.Replace("#",".");
			return fileName;
		}

		private string GetFilenameForProperties(WhichType whichType, XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Properties.html";
			return fileName;
		}

		private string GetFilenameForPropertyOverloads(XmlNode typeNode, XmlNode propertyNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string propertyName = (string)propertyNode.Attributes["name"].Value;
			string fileName = typeID.Substring(2) + propertyName + ".html";
			fileName = fileName.Replace("#",".");
			return fileName;
		}

		private string GetFilenameForProperty(XmlNode propertyNode)
		{
			string propertyID = (string)propertyNode.Attributes["id"].Value;
			string fileName = propertyID.Substring(2);

			int leftParenIndex = fileName.IndexOf('(');

			if (leftParenIndex != -1)
			{
				fileName = fileName.Substring(0, leftParenIndex);
			}

			if (propertyNode.Attributes["overload"] != null)
			{
				fileName += (string)propertyNode.Attributes["overload"].Value;
			}

			fileName += ".html";
			fileName = fileName.Replace("#",".");

			return fileName;
		}

		private string GetFilenameForMethods(WhichType whichType, XmlNode typeNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string fileName = typeID.Substring(2) + "Methods.html";
			return fileName;
		}

		private string GetFilenameForMethodOverloads(XmlNode typeNode, XmlNode methodNode)
		{
			string typeID = (string)typeNode.Attributes["id"].Value;
			string methodName = (string)methodNode.Attributes["name"].Value;
			string fileName = typeID.Substring(2) + "." + methodName + "_overloads.html";
			return fileName;
		}

		private string GetFilenameForMethod(XmlNode methodNode)
		{
			string methodID = (string)methodNode.Attributes["id"].Value;
			string fileName = methodID.Substring(2);

			int leftParenIndex = fileName.IndexOf('(');

			if (leftParenIndex != -1)
			{
				fileName = fileName.Substring(0, leftParenIndex);
			}

			fileName = fileName.Replace("#",".");

			if (methodNode.Attributes["overload"] != null)
			{
				fileName += "_overload_" + (string)methodNode.Attributes["overload"].Value;
			}

			fileName += ".html";

			return fileName;
		}

		/// <summary>
		/// This custom reader is used to load the XmlDocument. It removes elements that are not required *before* 
		/// they are loaded into memory, and hence lowers memory requirements significantly.
		/// </summary>
		private class FilteringXmlTextReader:XmlTextReader
		{
			object oNamespaceHierarchy;
			object oDocumentation;
			object oImplements;
			object oAttribute;

			public FilteringXmlTextReader(System.IO.Stream file):base(file)
			{
				base.WhitespaceHandling=WhitespaceHandling.None;
				oNamespaceHierarchy = base.NameTable.Add("namespaceHierarchy");
				oDocumentation = base.NameTable.Add("documentation");
				oImplements = base.NameTable.Add("implements");
				oAttribute = base.NameTable.Add("attribute");
			}
		
			private bool ShouldSkipElement()
			{
				return
					(
					base.Name.Equals(oNamespaceHierarchy)||
					base.Name.Equals(oDocumentation)||
					base.Name.Equals(oImplements)||
					base.Name.Equals(oAttribute)
					);
			}

			public override bool Read()
			{
				bool notEndOfDoc=base.Read();
				if (!notEndOfDoc) return false;
				while (notEndOfDoc && (base.NodeType == XmlNodeType.Element) && ShouldSkipElement() )
				{
					notEndOfDoc=SkipElement(this.Depth);
				}
				return notEndOfDoc;
			}

			private bool SkipElement(int startDepth)
			{
				if (base.IsEmptyElement) return base.Read();
				bool notEndOfDoc=true;
				while (notEndOfDoc)
				{
					notEndOfDoc=base.Read();
					if ((base.NodeType == XmlNodeType.EndElement) && (this.Depth==startDepth) ) 
						break;
				}
				if (notEndOfDoc) notEndOfDoc=base.Read();
				return notEndOfDoc;
			}

		}
	}
}
