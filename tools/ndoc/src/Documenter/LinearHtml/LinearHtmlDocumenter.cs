// LinearHtmlDocumenter.cs
// Copyright (C) 2003 Ryan Seghers
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

// To-do:
//   - handle method overloads some way or other
//   - handle type linking (property type link to original, only
//      if it's defined in this document)
//   - add an option to provide full details on members
//     (method arguments, remarks sections, ...)
//   - finish XSLT mode or remove it
//		- remove unused embedded resources
//   - make sorting and grouping simpler? (speed-complexity tradeoff)
//

// this allows switching between XmlDocument and XPathDocument
// (XPathDocument currently doesn't handle <code> nodes well)
#define USE_XML_DOCUMENT

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Reflection;

using NDoc.Core;
using NDoc.Core.Reflection;

namespace NDoc.Documenter.LinearHtml
{
	/// <summary>
	/// This creates a linear (serial, more printable) html file from an ndoc xml file. 
	/// This was designed and implemented with the intention that the html will be
	/// inserted into a Word document, but this could be useful for other scenarios.
	/// </summary>
	/// <remarks>
	/// <para><pre>
	/// The document produced is organized as follows:
	///    Namespaces List: a section listing namespaces and which assembly 
	///       they're from, and optionally their namespace summaries.
	///    Namespace: a section for each namespace
	///			Types List: a list of classes, interfaces, etc in the 
	///			    namespace, with their summaries.
	///			Type: Classes
	///			Type: Interfaces
	///			Type: Enumerations
	///			Type: Structs
	///			Type: Delegates
	///	</pre></para>
	///	<para>
	///	This class uses C#'s xml processing capabilities rather than xslt.
	///	This was more or less an experiment, and I'm not sure whether this
	///	is better than an xslt implementation or not.  The complexity might
	///	be similar, but I expect this implementation to be many times faster
	///	than an equivalent xslt implementation.
	///	</para>
	///	<para>
	///	This class writes a single linear html file, but traverses the xml
	///	document pretty much just once.  To do this, multiple XmlTextWriters
	///	are create such that they can be written to in any order.  Then at
	///	the end the memory buffers written by each text writer are copied
	///	to the output file in the appropriate order.
	///	</para> 
	///	<para>This has a Main for easier and faster test runs outside of NDoc.</para>
	/// </remarks>
	public class LinearHtmlDocumenter : BaseReflectionDocumenter
	{
		#region Fields

		/// <summary>The main navigator which keeps track of where we
		/// are in the document.</summary>
		XPathNavigator xPathNavigator;

		/// <summary>Writer for the first section, the namespace list.</summary>
		XmlTextWriter namespaceListWriter;

		/// <summary>A hashtable from namespace name to Hashtables which
		/// go from section name (Classes, Interfaces, etc) to XmlTextWriters
		/// for that section.</summary>
		Hashtable namespaceWriters; // namespace name -> Hashtables (of writers for each section)

		/// <summary>Hashtable from xml node name to section name. For example
		/// class to Classes.</summary>
		Hashtable namespaceSections; // xml node name -> section name

		/// <summary>
		/// The namespace sections in the order they will be emitted.
		/// </summary>
		string[] orderedNamespaceSections = { "interface",
				"enumeration", "delegate", "structure", "class" };

		/// <summary>
		/// A list of Type (class, interface) member types, to specify
		/// the order in which they should be rendered.
		/// </summary>
		string[] orderedMemberTypes = { "constructor", "field", "property", "method" };

		private Workspace workspace = null;

		// the Xslt is nowhere near good yet
		bool useXslt = false;

		/// <summary>This transform can be used for each type. This is incomplete.</summary>
		XslTransform typeTransform;

		#endregion

		#region Properties

		/// <summary>Cast to my type.</summary>
		private LinearHtmlDocumenterConfig MyConfig
		{
			get { return (LinearHtmlDocumenterConfig) Config; }
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Default constructor.
		/// </summary>
		public LinearHtmlDocumenter( LinearHtmlDocumenterConfig config ) : base( config )
		{
			namespaceWriters = new Hashtable();

			namespaceSections = new Hashtable();
			namespaceSections.Add("typeList", "Type List"); // writer for list of types in namespace
			namespaceSections.Add("class", "Classes");
			namespaceSections.Add("interface", "Interfaces");
			namespaceSections.Add("enumeration", "Enumerations");
			namespaceSections.Add("structure", "Structs");
			namespaceSections.Add("delegate", "Delegates");
		}

		#endregion

		#region Main Public API

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override string MainOutputFile 
		{ 
			get { return Path.Combine(this.WorkingPath, "linear.html"); } 
		}

		/// <summary>
		/// Load the specified NDoc Xml file into this object's memory.
		/// This is useful when this class is used outside of the context of NDoc.
		/// </summary>
		/// <param name="fileName">The NDoc Xml file to load.</param>
		/// <returns>bool - always true</returns>
		public bool Load(string fileName)
		{
			#if USE_XML_DOCUMENT
				XmlDocument doc = new XmlDocument();
				doc.Load(fileName);
			#else
				XPathDocument doc = new XPathDocument(fileName);
			#endif

			xPathNavigator = doc.CreateNavigator();
			return(true);
		}

		/// <summary>
		/// Load the specified NDoc Xml into this object's memory.
		/// This is useful when this class is used outside of the context of NDoc.
		/// </summary>
		/// <param name="s">The stream to load.</param>
		/// <returns>bool - always true</returns>
		public bool Load(Stream s)
		{
			#if USE_XML_DOCUMENT
				XmlDocument doc = new XmlDocument();
				doc.Load(s);
			#else
				XPathDocument doc = new XPathDocument(s);
			#endif

			xPathNavigator = doc.CreateNavigator();
			return(true);
		}

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override string CanBuild(Project project, bool checkInputOnly)
		{
			string result = base.CanBuild(project, checkInputOnly); 
			if (result != null) { return result; }
			if (checkInputOnly) { return null; }

			// test if output file is open
			string path = this.MainOutputFile;
			string temp = Path.Combine(MyConfig.OutputDirectory, "~lhtml.tmp");

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
				result = "The output file is probably open.\nPlease close it and try again.";
			}

			return result;
		}

		private string WorkingPath
		{ 
			get
			{ 
				if ( Path.IsPathRooted( MyConfig.OutputDirectory ) )
					return MyConfig.OutputDirectory; 

				return Path.GetFullPath( MyConfig.OutputDirectory );
			} 
		}

		/// <summary>See <see cref="IDocumenter"/>.</summary>
		public override void Build(Project project)
		{
			try
			{
				OnDocBuildingStep(0, "Initializing...");

				this.workspace = new LinearHtmlWorkspace( this.WorkingPath );
				workspace.Clean();
				workspace.Prepare();

				workspace.AddResourceDirectory( "xslt" );

// Define this when you want to edit the stylesheets
// without having to shutdown the application to rebuild.
#if NO_RESOURCES
				// copy all of the xslt source files into the workspace
				DirectoryInfo xsltSource = new DirectoryInfo( Path.GetFullPath(Path.Combine(
					System.Windows.Forms.Application.StartupPath, @"..\..\..\Documenter\LinearHtml\xslt") ) );

				foreach ( FileInfo f in xsltSource.GetFiles( "*.xslt" ) )
				{
					string destPath = Path.Combine( Path.Combine( workspace.ResourceDirectory, "xslt" ), f.Name );
					// change to allow overwrite if clean-up failed last time
					if (File.Exists(destPath)) File.SetAttributes( destPath, FileAttributes.Normal );
					f.CopyTo( destPath, true );
					// set attributes to allow delete later
					File.SetAttributes( destPath, FileAttributes.Normal );
				}

#else
				EmbeddedResources.WriteEmbeddedResources(this.GetType().Module.Assembly,
					"NDoc.Documenter.LinearHtml.xslt",
					Path.Combine(workspace.ResourceDirectory, "xslt"));
#endif

				// Create the html output directory if it doesn't exist.
				if (!Directory.Exists(MyConfig.OutputDirectory))
				{
					Directory.CreateDirectory(MyConfig.OutputDirectory);
				}

				// Write the embedded css files to the html output directory
				EmbeddedResources.WriteEmbeddedResources(this.GetType().Module.Assembly,
					"NDoc.Documenter.LinearHtml.css", MyConfig.OutputDirectory);

				// Write the external files (FilesToInclude) to the html output directory
				foreach( string srcFile in MyConfig.FilesToInclude.Split( '|' ) )
				{
					if ((srcFile == null) || (srcFile.Length == 0))
						continue;

					string dstFile = Path.Combine(MyConfig.OutputDirectory, Path.GetFileName(srcFile));
					File.Copy(srcFile, dstFile, true);
					File.SetAttributes(dstFile, FileAttributes.Archive);
				}

				OnDocBuildingStep(10, "Merging XML documentation...");

				// Will hold the name of the file name containing the XML doc
				string tempFileName = null;

				// Will hold the DOM representation of the XML doc
				XmlDocument xmlDocumentation = null;

				try 
				{
					// determine temp file name
					tempFileName = Path.GetTempFileName();
					// Let the Documenter base class do it's thing.
					MakeXmlFile(project, tempFileName);
					// Load the XML into DOM
					xmlDocumentation = new XmlDocument();
					xmlDocumentation.Load(tempFileName);
				} 
				finally 
				{
					if (tempFileName != null && File.Exists(tempFileName)) 
					{
						File.Delete(tempFileName);
					}
				}

#if USE_XML_DOCUMENT
				xPathNavigator = xmlDocumentation.CreateNavigator();
#else
				XmlTextWriter tmpWriter = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);
				xmlDocumentation.WriteTo(tmpWriter);
				tmpWriter.Flush();
				tmpWriter.BaseStream.Position = 0;
				this.Load(tmpWriter.BaseStream);
#endif

				// check for documentable types
				XmlNodeList typeNodes = xmlDocumentation.SelectNodes("/ndoc/assembly/module/namespace/*[name()!='documentation']");

				if (typeNodes.Count == 0)
				{
					throw new DocumenterException("There are no documentable types in this project.\n\nAny types that exist in the assemblies you are documenting have been excluded by the current visibility settings.\nFor example, you are attempting to document an internal class, but the 'DocumentInternals' visibility setting is set to False.\n\nNote: C# defaults to 'internal' if no accessibilty is specified, which is often the case for Console apps created in VS.NET...");
				}

				// create and write the html
				OnDocBuildingStep(50, "Generating HTML page...");
				MakeHtml(this.MainOutputFile);
				OnDocBuildingStep(100, "Done.");
				workspace.Clean();
			}
			catch(Exception ex)
			{
				throw new DocumenterException(ex.Message, ex);
			}
		}

		#endregion

		#region Namespace Writer Management Methods

		/// <summary>
		/// Setup any text writers.
		/// </summary>
		/// <returns></returns>
		private bool StartWriters()
		{
			// namespace list
			namespaceListWriter = new XmlTextWriter(new MemoryStream(), Encoding.UTF8);

			namespaceListWriter.WriteElementString("h1", "Namespace List");
			namespaceListWriter.WriteElementString("p", "The namespaces specified in this document are:");

			// table
			if (MyConfig.UseNamespaceDocSummaries)
			{
				string[] columnNames = { "Namespace", "Assembly", "Summary" };
				StartTable(namespaceListWriter, "NamespaceListTable", 600, columnNames);
			}
			else
			{
				string[] columnNames = { "Namespace", "Assembly" };
				StartTable(namespaceListWriter, "NamespaceListTable", 600, columnNames);
			}

			return(true);
		}

		/// <summary>
		/// Do whatever is neccesary to any writers before emitting html.
		/// </summary>
		/// <returns></returns>
		private bool EndWriters()
		{
			namespaceListWriter.WriteEndElement(); // table
			namespaceListWriter.Flush();

			return(true);
		}

		/// <summary>
		/// Close all writers.  They need to be re-created for the next build.
		/// </summary>
		/// <returns></returns>
		private bool DeleteWriters()
		{
			if (namespaceListWriter != null) namespaceListWriter.Close();
			namespaceListWriter = null;

			ArrayList nsSectionList = new ArrayList(orderedNamespaceSections);
			nsSectionList.Insert(0, "typeList"); // use this for the type list too

			foreach(string namespaceName in namespaceWriters.Keys)
			{
				Hashtable nsSectionWriters = (Hashtable)namespaceWriters[namespaceName];

				foreach(string sectionKey in nsSectionList)
				{
					string sectionName = (string)namespaceSections[sectionKey];
					XmlTextWriter xtw = (XmlTextWriter)nsSectionWriters[sectionName];
					if (xtw != null) 
					{
						xtw.Close();
						nsSectionWriters.Remove(sectionName);
					}
				}

				nsSectionWriters.Clear();
			}

			namespaceWriters.Clear();

			return(true);
		}

		/// <summary>
		/// Create a namespace section writer if one doesn't already exist
		/// for the specified namespace and section.
		/// </summary>
		/// <param name="namespaceName">C# namespace name, not xml namespace.</param>
		/// <param name="sectionName">The section name, such as Classes.</param>
		void StartNamespaceSectionWriter(string namespaceName, string sectionName)
		{
			if (!namespaceWriters.ContainsKey(namespaceName)) 
				namespaceWriters.Add(namespaceName, new Hashtable());

			Hashtable nsSectionWriters = (Hashtable)namespaceWriters[namespaceName];
			if (!nsSectionWriters.ContainsKey(sectionName))
			{
				Trace.WriteLine(String.Format("Added section writer: ns {0} section {1}", 
					namespaceName, sectionName));
				nsSectionWriters.Add(sectionName, new XmlTextWriter(new MemoryStream(),
					Encoding.UTF8));

				XmlTextWriter xtw = (XmlTextWriter)nsSectionWriters[sectionName];
				//xtw.Formatting = Formatting.Indented;
				xtw.Indentation = 4;

				if (sectionName.Equals("Type List"))
				{
					// make this a potential link target (for the namespace)
					xtw.WriteStartElement("h2");
					xtw.WriteAttributeString("id", GetNamespaceHtmlId(namespaceName));
					xtw.WriteString(String.Format("{0} {1}", namespaceName, sectionName));
					//xtw.WriteString(sectionName);
					xtw.WriteEndElement();
				}
				else
				{
					xtw.WriteElementString("h2", String.Format("{0} {1}", namespaceName, sectionName));
					//xtw.WriteElementString("h2", sectionName);
				}
			}
		}

		#endregion

		#region System Type/Access Utility Methods

		/// <summary>
		/// Convert a full type name like System.Int32 to the more simple version
		/// "int".
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToSimpleType(string s)
		{
			if ((s == null) || (s == string.Empty)) return(string.Empty);

			Type type = Type.GetType(s);
			if (type == null) return(TypeBaseName(s));

			TypeCode code = Type.GetTypeCode(type);
			if (code.Equals(TypeCode.Object)) return(TypeBaseName(s));

			return(code.ToString());
		}

		/// <summary>
		/// Convert an access string as in the xml (Public, Family, etc) to
		/// one like we want for declarations (public, protected, etc).
		/// </summary>
		/// <param name="typeAccess"></param>
		/// <returns></returns>
		public static string ToAccessDeclaration(string typeAccess)
		{
			if ((typeAccess == null) || (typeAccess == string.Empty)) return(string.Empty);
			string lcTypeAccess = Char.ToLower(typeAccess[0]) + typeAccess.Substring(1);
			if (lcTypeAccess.Equals("family")) return("protected");
			else return(lcTypeAccess);
		}

		/// <summary>
		/// Return the base of the input type name.  For example the base of 
		/// System.String is String.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static string TypeBaseName(string typeName)
		{
			if ((typeName == null) || (typeName == string.Empty)) return(string.Empty);
			if (typeName.IndexOf(".") >= 0)
				return(typeName.Substring(typeName.LastIndexOf(".") + 1));
			else return(typeName);
		}

		#endregion

		#region Xml Utility Methods

		/// <summary>
		/// Get the children of the current node which have the specified
		/// localName.  
		/// </summary>
		/// <remarks>
		/// This is motivated by the fact that XPathNavigator.SelectChildren()
		/// apparently looks at all descendants.
		/// </remarks>
		/// <param name="localName">The localname to select.</param>
		/// <param name="nav">The XPathNavigator.</param>
		/// <returns>A new ArrayList of XPathNavigators.</returns>
		ArrayList GetChildren(XPathNavigator nav, string localName)
		{
			ArrayList list = new ArrayList();

			XPathNavigator children = nav.Clone();
			children.MoveToFirstChild();
			do
			{
				if (children.LocalName.Equals(localName))
				{
					list.Add(children.Clone());
				}
			} while(children.MoveToNext());

			return(list);
		}

		/// <summary>
		/// Get the descendants of the current node which have the specified
		/// localName.  This just provides a different iteration style
		/// from XPathNavigator.SelectDescendants().
		/// </summary>
		/// <param name="localName">The localname to select.</param>
		/// <param name="nav">The XPathNavigator.</param>
		/// <returns>A new ArrayList of XPathNavigators.</returns>
		ArrayList GetDescendants(XPathNavigator nav, string localName)
		{
			ArrayList list = new ArrayList();

			XPathNodeIterator iter = nav.SelectDescendants(localName, "", false);
			while(iter.MoveNext())
			{
				list.Add(iter.Current.Clone());
			}

			return(list);
		}

		/// <summary>
		/// Returns a SortedList which links each child node's attribute value (sorted, of course)
		/// to an XPathNavigator pointing to that node. This selects child nodes
		/// based on node local name.  This just looks at immediate children, not
		/// all descendants.
		/// </summary>
		/// <remarks>
		/// Surprisingly, XPathNavigator.SelectChildren() apparently visits all
		/// descendants, despite the presence of XPathNavigator.SelectDescendants().
		/// </remarks>
		/// <param name="attrName">The attribute to sort on.</param>
		/// <param name="nav">The parent of the children to index.</param>
		/// <param name="localName">The localName of child nodes to select.</param>
		/// <returns>The SortedList of child node attribute values to 
		/// XPathNavigators.</returns>
		SortedList GetSortedChildren(XPathNavigator nav, string localName, string attrName)
		{
			SortedList sortedList = new SortedList();

			XPathNavigator children = nav.Clone();
			children.MoveToFirstChild();
			do
			{
				if (children.LocalName.Equals(localName))
				{
					string attrVal = children.GetAttribute(attrName, string.Empty);
					sortedList.Add(attrVal, children.Clone());
				}
			} while(children.MoveToNext());

			return(sortedList);
		}

		/// <summary>
		/// Fix code node such that it will be rendered correctly (using pre).
		/// </summary>
		/// <param name="topNode"></param>
		private void FixCodeNodes(XmlNode topNode)
		{
			foreach(XmlNode codeNode in topNode.SelectNodes("descendant::code"))
			{
				codeNode.InnerXml = "<pre class=\"code\">" + codeNode.InnerXml + "</pre>";
			}
		}

		/// <summary>
		/// Fix any code nodes under the specified navigator, and return the node's
		/// inner Xml. 
		/// </summary>
		/// <param name="nav"></param>
		/// <returns></returns>
		private string GetNodeXmlFixCode(XPathNavigator nav)
		{
			if (nav == null) return(string.Empty);

			// want the XmlNode if possible (depends on whether nav came from
			// XmlDocument or XPathDocument).
			// Can't seem to get raw xml from the navigator in the XPathDocument case.
			// Don't know another way to test the cast, and it must be slow on exception
			string s = string.Empty;
			try
			{
				XmlNode n = ((IHasXmlNode)nav).GetNode();
				FixCodeNodes(n); // change <code> to <pre class="code">
				s = n.InnerXml;
			}
			catch(Exception) 
			{
				s = nav.Value;
			}
			return(s);
		}

		private void WriteNodeFixCode(XmlTextWriter xtw, XPathNavigator nav,
			string elemType)
		{
			xtw.WriteStartElement(elemType);
			xtw.WriteRaw(GetNodeXmlFixCode(nav));
			xtw.WriteEndElement();
		}

	/* doesn't work because can't modify node via XPathNavigator
				/// <summary>
				/// Fix code nodes such that it will be rendered correctly (using pre).
				/// </summary>
				/// <param name="topNode"></param>
				private void FixCodeNodes(XPathNavigator nav)
				{
					XPathNodeIterator iter = nav.SelectDescendants("code", string.Empty, true);
					while(iter.MoveNext())
					{
						XPathNavigator n = iter.Current;
						n.Value = "<pre class=\"code\">" + n.Value + "</pre>";
					}
				}
		*/

		/// <summary>
		/// Return a new XPathNavigator pointing to the first descendant node
		/// with the specified name.
		/// </summary>
		/// <param name="nodeName">The node name string.</param>
		/// <param name="startNavigator">Initial node to start search from.</param>
		/// <returns>An XPathNavigator pointing to the specified descendant, or null
		/// for not found.</returns>
		XPathNavigator GetDescendantNodeWithName(XPathNavigator startNavigator, string nodeName)
		{
			XPathNodeIterator xni = startNavigator.SelectDescendants(nodeName, "", false);
			xni.MoveNext();
			if (xni.Current.ComparePosition(startNavigator) == XmlNodeOrder.Same)
				return(null);
			else return(xni.Current);
		}

		/// <summary>
		/// Return a new XPathNavigator pointing to the first child node
		/// of the specified name.
		/// </summary>
		/// <param name="nodeName">The node name string.</param>
		/// <param name="startNavigator">Initial node to start search from.</param>
		/// <returns>An XPathNavigator pointing to the specified child, or null
		/// for not found.</returns>
		XPathNavigator GetChildNodeWithName(XPathNavigator startNavigator, string nodeName)
		{
			XPathNavigator children = startNavigator.Clone();
			children.MoveToFirstChild();
			do
			{
				if (children.LocalName.Equals(nodeName)) return(children);
			} while(children.MoveToNext());
			return(null);
		}

		/// <summary>
		/// For debugging, display the node local names starting from
		/// a particular node.
		/// </summary>
		/// <param name="nav">The start point.</param>
		/// <param name="prefix">An indentation prefix for the display.</param>
		void DumpNavTree(XPathNavigator nav, string prefix)
		{
			XPathNavigator n = nav.Clone();
			Console.WriteLine("{0} {1}", prefix, n.LocalName);

			// display children of specified node, recursively
			n.MoveToFirstChild();
			do
			{
				if (n.HasChildren) DumpNavTree(n, prefix + "    ");
			} while(n.MoveToNext());
		}

		#endregion

		#region Html Utility Methods

		/// <summary>
		/// Turn a namespace name into an html id we can use for links.
		/// </summary>
		/// <param name="namespaceName"></param>
		/// <returns></returns>
		private string GetNamespaceHtmlId(string namespaceName)
		{
			return(namespaceName + "_nsId");
		}

		/// <summary>
		/// Turn a type name into an html id we can use for links.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private string GetTypeHtmlId(string typeName)
		{
			return(typeName + "_typeId");
		}

		/// <summary>
		/// Create a string which wraps the input string to make it a link
		/// to the specified namespace.
		/// </summary>
		/// <param name="namespaceName">The string to wrap.</param>
		/// <returns>The wrapped string.</returns>
		private string NamespaceLinkReferenceWrap(string namespaceName)
		{
			// what tag should I use?  I just want the class
			return("<A href=\"#" + GetNamespaceHtmlId(namespaceName) 
				+ "\">" + namespaceName + "</A>");
		}

		/// <summary>
		/// Create a string which wraps the input string to make it a link
		/// to the specified type.
		/// </summary>
		/// <param name="typeName">The string to wrap.</param>
		/// <returns>The wrapped string.</returns>
		private string TypeLinkReferenceWrap(string typeName)
		{
			// what tag should I use?  I just want the class
			return("<A href=\"#" + GetTypeHtmlId(typeName) 
				+ "\">" + typeName + "</A>");
		}

		/// <summary>
		/// Create a string which wraps the input string with a span tag
		/// with a particular html class attribute.
		/// </summary>
		/// <param name="s">The string to wrap.</param>
		/// <returns>The wrapped string.</returns>
		private string TypeRefWrap(string s)
		{
			// what tag should I use?  I just want the class
			return("<span class=\"typeRef\">" + s + "</span>");
		}

		/// <summary>
		/// Create a string which wraps the input string with a span tag
		/// with a particular html class attribute.
		/// </summary>
		/// <param name="s">The string to wrap.</param>
		/// <returns>The wrapped string.</returns>
		private string KeyWrap(string s)
		{
			// what tag should I use?  I just want the class
			return("<span class=\"keyWord\">" + s + "</span>");
		}

		/// <summary>
		/// Write the starting tags for a table to the specified writer.
		/// </summary>
		/// <param name="xtw"></param>
		/// <param name="id">The table id to put in the html.</param>
		/// <param name="width">The width (pixels) of the table.</param>
		/// <param name="columnNames">An array of column names to use
		/// in the header row. This also sets the number of columns.</param>
		/// <returns>bool - true</returns>
		private bool StartTable(XmlTextWriter xtw, string id, int width, 
			string[] columnNames)
		{
			xtw.WriteStartElement("TABLE");
			xtw.WriteAttributeString("class", "dtTable");
			xtw.WriteAttributeString("id", id);
			xtw.WriteAttributeString("cellSpacing", "1");
			xtw.WriteAttributeString("cellPadding", "1");
			xtw.WriteAttributeString("width", String.Format("{0}", width));
			xtw.WriteAttributeString("border", "1");
			xtw.WriteStartElement("TR");
			foreach(string colName in columnNames)
			{
				xtw.WriteStartElement("TH");
				xtw.WriteAttributeString("style", "background:#FFFFCA");
				xtw.WriteString(colName);
				xtw.WriteEndElement();
			}
			xtw.WriteEndElement(); // TR
			return(true);
		}

		/// <summary>
		/// Write a table entry (one row) consisting of one or more columns
		/// of text.
		/// </summary>
		/// <param name="xtw"></param>
		/// <param name="args">The strings to write.</param>
		private void AddTableEntry(XmlTextWriter xtw, params object[] args)
		{
			if ((args != null) && (args.Length > 0))
			{
				xtw.WriteStartElement("TR");
				foreach(string s in args)
				{
					xtw.WriteElementString("TD", s);
				}
				xtw.WriteEndElement(); // TR
			}
		}

		/// <summary>
		/// Write a table entry (one row) consisting of one or more columns
		/// of text, written Raw.
		/// </summary>
		/// <param name="xtw"></param>
		/// <param name="args">The strings to write.</param>
		private void AddTableEntryRaw(XmlTextWriter xtw, params object[] args)
		{
			if ((args != null) && (args.Length > 0))
			{
				xtw.WriteStartElement("TR");
				foreach(string s in args)
				{
					xtw.WriteStartElement("TD");
					xtw.WriteRaw(s);
					xtw.WriteEndElement();
				}
				xtw.WriteEndElement(); // TR
			}
		}

		/// <summary>
		/// End a table. This is provided for symmetry, and in case there's
		/// something else I have to write in tables in the future.
		/// </summary>
		/// <param name="xtw"></param>
		private void EndTable(XmlTextWriter xtw)
		{
			xtw.WriteEndElement(); // TABLE
		}

		#endregion

		#region Make Html

		#region Top level, assembly, module, and namespace

		/// <summary>
		/// Build and emit the html document from the loaded NDoc Xml document.
		/// </summary>
		/// <returns></returns>
		private bool MakeHtml(string outputFileName)
		{
			StartWriters();
			XPathNavigator assyNav = GetDescendantNodeWithName(xPathNavigator, "assembly");

			// for each assembly...
			do 
			{
				if (assyNav.Name == "assembly")
				{
					MakeHtmlForAssembly(assyNav);
				}
				else
				{
					Trace.WriteLine(String.Format("Skipping non-assembly node name {0}", 
						assyNav.Name));
				}
			} while(assyNav.MoveToNext());

			EndWriters(); // prep for emit
			EmitHtml(outputFileName);
			DeleteWriters(); // close them so re-build doesn't accumulate html

			return(true);
		}

		/// <summary>
		/// Do the build operations given that the specified XPathNavigator is pointing to
		/// an assembly node.
		/// </summary>
		/// <param name="nav">The XPathNavigator pointing to a node of type
		/// appropriate for this method.</param>
		void MakeHtmlForAssembly(XPathNavigator nav)
		{
			string assemblyName = nav.GetAttribute("name", "");
			string assemblyVersion = nav.GetAttribute("version", "");
			Console.WriteLine("Assembly: {0}, {1}", assemblyName, assemblyVersion);
			nav.MoveToFirstChild();

			// foreach module...
			do 
			{
				Trace.WriteLine(String.Format("Making HTML for assembly {0}", assemblyName));
				MakeHtmlForModule(nav, assemblyName, assemblyVersion);
			} while(nav.MoveToNext());

			nav.MoveToParent();
		}

		/// <summary>
		/// Do the build operations given that the specified <see cref="XPathNavigator" /> 
		/// is pointing to a module node.
		/// </summary>
		/// <param name="nav">The <see cref="XPathNavigator" /> pointing to a 
		/// node of a type that's appropriate for this method.</param>
        /// <param name="assemblyName">The name of the assembly containing the module.</param>
        /// <param name="assemblyVersion">The version of the assembly containing the module.</param>
        void MakeHtmlForModule(XPathNavigator nav, string assemblyName, string assemblyVersion)
		{
			//string moduleName = nav.GetAttribute("name", "");
			Console.WriteLine("Module: {0}", nav.GetAttribute("name", ""));
			nav.MoveToFirstChild();

			// foreach namespace
			do 
			{
				MakeHtmlForNamespace(nav, assemblyName, assemblyVersion);
			} while(nav.MoveToNext());

			nav.MoveToParent();
		}

		/// <summary>
		/// Do the build operations given that the specified <see cref="XPathNavigator" /> 
		/// is pointing to a namespace node.
		/// </summary>
		/// <param name="nav">The <see cref="XPathNavigator" /> pointing to a 
		/// node of a type that's appropriate for this method.</param>
		/// <param name="assemblyName">The name of the assembly containing this namespace.</param>
		/// <param name="assemblyVersion">The version of the assembly containing this namespace.</param>
		void MakeHtmlForNamespace(XPathNavigator nav, string assemblyName, string assemblyVersion)
		{
			string namespaceName = nav.GetAttribute("name", "");

			// skip this namespace based on regex
			if ((MyConfig.NamespaceExcludeRegexp != null) 
				&& (MyConfig.NamespaceExcludeRegexp.Length > 0))
			{
				Regex nsReject = new Regex(MyConfig.NamespaceExcludeRegexp);

				if (nsReject.IsMatch(namespaceName))
				{
					Console.WriteLine("Rejecting namespace {0} by regexp", namespaceName);
					return;
				}
			}

			Console.WriteLine("Namespace: {0}", namespaceName);

			//
			// this goes in list of namespaces
			//
			string assemblyString = assemblyName;
			if ((assemblyVersion != null) && (assemblyVersion.Length > 0))
			{
				Version v = new Version(assemblyVersion);
				string vString = String.Format("{0}.{1}.{2}.*", v.Major, v.Minor, v.Build);
				assemblyString = assemblyName + " Version " + vString;
			}

			// get summary
			XPathNavigator documentationNav = GetChildNodeWithName(nav, "documentation");
			XPathNavigator summaryNav = null;
			if (documentationNav != null)
			{
				summaryNav = GetChildNodeWithName(documentationNav, "summary");
			}
			string namespaceSummary = string.Empty;
			if (summaryNav != null) namespaceSummary = summaryNav.Value;

			if (MyConfig.UseNamespaceDocSummaries)
			{
				AddTableEntryRaw(namespaceListWriter, NamespaceLinkReferenceWrap(namespaceName), 
					assemblyString, namespaceSummary);
			}
			else
			{
				AddTableEntryRaw(namespaceListWriter, NamespaceLinkReferenceWrap(namespaceName), 
					assemblyString);
			}

			//
			// write a list of types in this namespace
			//
			MakeHtmlTypeList(nav, namespaceName);

			//
			// Types in namespace
			//
			nav.MoveToFirstChild(); // move into namespace children

			// foreach Type (class, delegate, interface)...
			do 
			{
				if (TypeMatchesIncludeRegexp(nav)) MakeHtmlForType(nav, namespaceName);
			} while(nav.MoveToNext());

			nav.MoveToParent();
		}

		/// <summary>
		/// Write some html containing the list of types in this namespace.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="namespaceName"></param>
		void MakeHtmlTypeList(XPathNavigator nav, string namespaceName)
		{
			// get a writer for this namespace's type list
			StartNamespaceSectionWriter(namespaceName, "Type List");
			Hashtable nsSectionWriters = (Hashtable)namespaceWriters[namespaceName];
			XmlTextWriter xtw = (XmlTextWriter)nsSectionWriters["Type List"];

			string[] colNames = { "Type", "Summary" };

			// foreach namespace section ( "Classes", "Interfaces", etc)
			foreach(string sectionName in orderedNamespaceSections)
			{
				// alphabetize
				SortedList sortedList = GetSortedChildren(nav, sectionName, "name");

				if (sortedList.Count > 0)
				{
					xtw.WriteElementString("h3", (string)namespaceSections[sectionName]);
					this.StartTable(xtw, namespaceName + "_TypeList_" + sectionName, 600, colNames);
					foreach(DictionaryEntry entry in sortedList)
					{
						string typeName = (string)entry.Key;
						XPathNavigator n = (XPathNavigator)entry.Value;

						if (TypeMatchesIncludeRegexp(n))
						{
							//string typeName = n.GetAttribute("name", string.Empty);
							XPathNavigator summaryNav = GetDescendantNodeWithName(n, "summary");
							string summary = string.Empty;
							if (summaryNav != null) summary = summaryNav.Value;
							AddTableEntryRaw(xtw, this.TypeLinkReferenceWrap(typeName), summary);
						}
					} 
					this.EndTable(xtw);
				}
			}
		}

		/// <summary>
		/// This checks whether the Type represented by the specified node
		/// matches the include type regexp.
		/// </summary>
		/// <param name="nav">The XPathNavigator pointing to the type node.</param>
		/// <returns>bool - true for should be included, false for should be
		/// excluded.</returns>
		bool TypeMatchesIncludeRegexp(XPathNavigator nav)
		{
			// skip this namespace based on regex
			if ((MyConfig.TypeIncludeRegexp != null) 
				&& (MyConfig.TypeIncludeRegexp.Length > 0))
			{
				string nodeName = nav.GetAttribute("name", "");
				Regex typeIncludeRegex = new Regex(MyConfig.TypeIncludeRegexp);

				if (typeIncludeRegex.IsMatch(nodeName))
				{
					Console.WriteLine("Including node {0} by regexp", nodeName);
					return(true);
				}
				else
				{
					Console.WriteLine("Excluding node {0} by regexp", nodeName);
					return(false);
				}
			}
			else return(true);
		}

		#endregion

		#region Type

		/// <summary>
		/// Builds html for a Type.  An Type here is a class, struct, interface, etc.
		/// </summary>
		/// <param name="nav">The XPathNavigator pointing to the type node.</param>
		/// <param name="namespaceName">The namespace containing this type.</param>
		void MakeHtmlForType(XPathNavigator nav, string namespaceName)
		{
			string nodeName = nav.GetAttribute("name", "");
			string nodeType = nav.LocalName;
			Trace.WriteLine(String.Format("MakeHtmlForType: Visiting Type Node: {0}: {1}", nodeType, nodeName));

			if (namespaceSections.ContainsKey(nodeType))
			{
				// write to appropriate writer
				string sectionName = (string)namespaceSections[nodeType];
				StartNamespaceSectionWriter(namespaceName, sectionName);
					
				// now write members
				Hashtable nsSectionWriters = (Hashtable)namespaceWriters[namespaceName];
				XmlTextWriter xtw = (XmlTextWriter)nsSectionWriters[sectionName];

				if (useXslt)
				{
					MakeHtmlForTypeUsingXslt(nav, xtw, namespaceName);
				}
				else
				{
					MakeHtmlForTypeUsingCs(nav, xtw, namespaceName);
				}
			}
			else 
			{
				if (!nodeType.Equals("summary"))
					Console.WriteLine("Warn: MakeHtmlForType: Unknown section for node name {0}", 
						nav.LocalName);
			}
		}

		/// <summary>
		/// Use Xslt transform to document this type.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="xtw"></param>
		/// <param name="namespaceName"></param>
		void MakeHtmlForTypeUsingXslt(XPathNavigator nav, XmlTextWriter xtw, string namespaceName)
		{
			//string nodeName = nav.GetAttribute("name", "");
			//string nodeType = nav.LocalName;
			string typeId = nav.GetAttribute("id", "");

			// create transform if it hasn't already been created
			if (typeTransform == null)
			{
				string fileName = "linearHtml.xslt";
				//string transformPath = Path.Combine(Path.Combine(resourceDirectory, "xslt"), filename);
				string transformPath = Path.Combine(@"C:\VSProjects\Util\ndoc\src\Documenter\LinearHtml\xslt", 
					fileName);

				typeTransform = new XslTransform();
				typeTransform.Load(transformPath);
			}

			// execute the transform
			XsltArgumentList arguments = new XsltArgumentList();
			arguments.AddParam("namespace", String.Empty, namespaceName);
			arguments.AddParam("type-id", String.Empty, typeId);
			arguments.AddParam("includeHierarchy", String.Empty, MyConfig.IncludeHierarchy);
			typeTransform.Transform(nav, arguments, xtw);
		}

		/// <summary>
		/// Document the current node's type using C#.
		/// </summary>
		/// <param name="nav"></param>
		/// <param name="xtw"></param>
		/// <param name="namespaceName"></param>
		void MakeHtmlForTypeUsingCs(XPathNavigator nav, XmlTextWriter xtw, string namespaceName)
		{
			string nodeName = nav.GetAttribute("name", "");
			string nodeType = nav.LocalName;
			string capsItemType = char.ToUpper(nodeType[0]) + nodeType.Substring(1);

			// put in an h3 anchor for links to this type
			xtw.WriteStartElement("h3");
			xtw.WriteStartElement("a");
			xtw.WriteAttributeString("name", GetTypeHtmlId(nodeName));
			xtw.WriteEndElement();
			xtw.WriteString(String.Format("{0} {1}", nodeName, capsItemType));
			xtw.WriteEndElement(); // h3

			//
			// collect navigators to various members by category
			//
			Hashtable memberTypeHt = new Hashtable(); // memberType -> Hashtable (id -> navigator)
			nav.MoveToFirstChild();
			Hashtable navTable;

			do 
			{
				// each member of type
				string memberType = nav.LocalName;
				string memberId = nav.GetAttribute("id", "");

				if (!memberTypeHt.ContainsKey(memberType)) 
				{
					//Console.WriteLine("Add member type {0}", memberType);
					memberTypeHt.Add(memberType, new Hashtable());
				}
				navTable = (Hashtable)memberTypeHt[memberType];
				if (!navTable.ContainsKey(memberId)) navTable.Add(memberId, nav.Clone());
			} while(nav.MoveToNext());

			nav.MoveToParent();

			//
			// Summary/declaration
			//
			xtw.WriteElementString("h4", "Summary");
			string declarationString = MakeHtmlTypeDeclaration(nav);

			xtw.WriteRaw("<p><code>" + declarationString + "</code></p>");

			//
			// documentation/summary
			//
			XPathNavigator remarksNav = null;
			if (memberTypeHt.ContainsKey("documentation"))
			{
				navTable = (Hashtable)memberTypeHt["documentation"];
				XPathNavigator nav2 = (XPathNavigator)navTable[String.Empty];
				XPathNavigator summaryNav = GetDescendantNodeWithName(nav2, "summary");
				remarksNav = GetDescendantNodeWithName(nav2, "remarks");

				if (summaryNav != null) WriteNodeFixCode(xtw, summaryNav, "p");
			}

			// attributes
			if (memberTypeHt.ContainsKey("attribute"))
			{
				navTable = (Hashtable)memberTypeHt["attribute"];
				StringBuilder sb = new StringBuilder("This type has the following attributes: ");

				bool first = true;
				foreach(string memberId in navTable.Keys)
				{
					if (!first) { sb.Append(", "); first = false; }
					//XPathNavigator nav2 = (XPathNavigator)navTable[memberId];
					//this.DumpNavTree(nav2, "    ");
					string tmps = ((XPathNavigator)navTable[memberId]).GetAttribute("name", "");
					sb.Append(tmps);
				}
				xtw.WriteElementString("p", sb.ToString());
			}

			//
			// documentation/remarks
			//
			if (remarksNav != null)
			{
				xtw.WriteElementString("h4", "Remarks");
				WriteNodeFixCode(xtw, remarksNav, "p");
			}

			//
			// the members (properties, methods, fields, etc) in the type
			//
			if (nodeType == "enumeration")
			{
				// enumerations
				string memberType = "field"; // members are all fields in enumerations

				if (memberTypeHt.ContainsKey(memberType))
				{
					xtw.WriteElementString("h4", String.Format("{0} Members", "Enumeration"));
					string[] columnNames = { "Field", "Summary" };
					StartTable(xtw, memberType + "_TableId_" + nodeName, 600, columnNames);

					//
					// create a table entry for each member of this Type
					//
					navTable = (Hashtable)memberTypeHt[memberType]; // memberId -> navigator

					// sort by member id (approximately the member name?)
					SortedList sortedMemberIds = new SortedList(navTable);

					foreach(string memberId in sortedMemberIds.Keys)
					{
						XPathNavigator nav2 = (XPathNavigator)navTable[memberId];
						string memberName = nav2.GetAttribute("name", "");

						// get summary
						string summaryString = string.Empty;
						XPathNavigator summaryNav = GetDescendantNodeWithName(nav2, "summary");
						if (summaryNav != null) summaryString = summaryNav.Value;
						
						this.AddTableEntry(xtw, memberName, summaryString);
					}

					EndTable(xtw);
				}
				else
				{
					// hmm, an enumeration with no fields?
					Console.WriteLine("Error: LinearHtml: MakeHtmlForTypeUsingCs: No fields in enumeration {0}?",
						nodeName);
				}
			}
			else
			{
				// struct, class, etc.

				//
				// member types which use name/access/summary table
				//
				foreach(string memberType in orderedMemberTypes) // memberType in constructor, field, property...
				{
					if (memberTypeHt.ContainsKey(memberType))
					{
						string capsMemberType = char.ToUpper(memberType[0]) + memberType.Substring(1);
						xtw.WriteElementString("h4", String.Format("{0} Members", capsMemberType));
						navTable = (Hashtable)memberTypeHt[memberType]; // memberId -> navigator
						// sort by member id (approximately the member name?)
						SortedList sortedMemberIds = new SortedList(navTable);

						if (MyConfig.IncludeTypeMemberDetails && 
							(memberType.Equals("method") || memberType.Equals("constructor")))
						{
							// method, with details
							foreach(string memberId in sortedMemberIds.Keys)
							{
								XPathNavigator nav2 = (XPathNavigator)navTable[memberId];
								MakeHtmlDetailsForMethod(nodeName, memberType, nav2, xtw);
							}
						}
						else
						{
							// not a method, or no details
							string[] columnNames = { "Name", "Access", "Summary" };
							StartTable(xtw, memberType + "_TableId_" + nodeName, 600, columnNames);

							//
							// create a table entry for each member of this Type
							//
							foreach(string memberId in sortedMemberIds.Keys)
							{
								XPathNavigator nav2 = (XPathNavigator)navTable[memberId];
								MakeHtmlForTypeMember(nodeName, memberType, nav2, xtw);
							}

							EndTable(xtw);
						}
					}
				}
			}
		}

		#endregion

		#region Type Members, methods

		/// <summary>
		/// Make (and write) html for a Type (class, interface, ...) member, such
		/// as a property, field, etc.
		/// </summary>
		/// <param name="parentTypeName">The Type's name.</param>
		/// <param name="memberType">The type of the member (property, field, 
		/// method, etc)</param>
		/// <param name="nav">Pointing to the member's node.</param>
		/// <param name="xtw"></param>
		void MakeHtmlForTypeMember(string parentTypeName, string memberType, 
			XPathNavigator nav, XmlTextWriter xtw)
		{
			bool includeMethodSignature = MyConfig.MethodParametersInTable;
			string memberAccess = nav.GetAttribute("access", "");
			string memberName = nav.GetAttribute("name", "");
			string typeName = nav.GetAttribute("type", "");
			string typeBaseName = TypeBaseName(typeName);
			string declaringType = nav.GetAttribute("declaringType", "");

			XPathNavigator summaryNav = GetDescendantNodeWithName(nav, "summary");
			//DumpNavTree(summaryNav, "    ");
			XPathNavigator remarksNav = GetDescendantNodeWithName(nav, "remarks");

			string summaryString = string.Empty;
			if (summaryNav != null) summaryString = summaryNav.Value;
			string remarksString = string.Empty;
			if (MyConfig.IncludeTypeMemberDetails && (remarksNav != null))
				remarksString = "<br/><br/>" + remarksNav.Value;

			//
			// create a string for the name column
			//
			string nameString = memberName;
			string args = "";
			switch(memberType)
			{
				case "field":
					nameString = KeyWrap(memberName) + TypeRefWrap(" : " + typeBaseName);
					break;
				case "property":
					nameString = KeyWrap(memberName) + TypeRefWrap(" : " + typeBaseName);
					break;
				case "method":
					typeBaseName = TypeBaseName(nav.GetAttribute("returnType", ""));
					if (includeMethodSignature) args = MakeMethodParametersString(nav);
					nameString = KeyWrap(memberName + "(" + args + ")") + TypeRefWrap(" : " + typeBaseName);
					remarksString = string.Empty; // don't include remarks for methods
					break;
				case "constructor":
					if (includeMethodSignature) args = MakeMethodParametersString(nav);
					nameString = KeyWrap(parentTypeName + "(" + args + ")");
					remarksString = string.Empty; // don't include remarks for methods
					break;
			}

			//
			// write the member if it isn't from a System class
			//
			if (declaringType.IndexOf("System") != 0)
			{
				xtw.WriteStartElement("TR");
				xtw.WriteStartElement("TD");
				xtw.WriteRaw(nameString);
				xtw.WriteEndElement();
				xtw.WriteElementString("TD", ToAccessDeclaration(memberAccess));

				if (declaringType.Length > 0)
				{
					//xtw.WriteElementString("TD", "<em>(from " + declaringType + ")</em> " + summaryNav.Value);

					// declared by an ancestor
					xtw.WriteStartElement("TD");
					xtw.WriteStartElement("em");
					xtw.WriteRaw("(from " + TypeRefWrap(declaringType) + ")");
					xtw.WriteEndElement(); // em
					xtw.WriteString(" ");
					xtw.WriteString(summaryString);
					if (!remarksString.Equals(string.Empty)) xtw.WriteRaw(remarksString);
					xtw.WriteEndElement(); // TD
				}
				else
				{
					xtw.WriteStartElement("TD");
					xtw.WriteString(summaryString);
					if (!remarksString.Equals(string.Empty)) xtw.WriteRaw(remarksString);
					xtw.WriteEndElement(); // TD
				}

				xtw.WriteEndElement();
			}
		}

		/// <summary>
		/// Write html for a single method.
		/// </summary>
		/// <param name="parentTypeName"></param>
		/// <param name="memberType"></param>
		/// <param name="nav"></param>
		/// <param name="xtw"></param>
		void MakeHtmlDetailsForMethod(string parentTypeName, string memberType, 
			  XPathNavigator nav, XmlTextWriter xtw)
		{
			// get summary and remarks strings
			XPathNavigator summaryNav = GetDescendantNodeWithName(nav, "summary");
			XPathNavigator remarksNav = GetDescendantNodeWithName(nav, "remarks");

			xtw.WriteRaw(String.Format("<b>{0}</b>", MakeMethodDeclaration(nav, parentTypeName)));
			xtw.WriteStartElement("div");
			xtw.WriteAttributeString("class", "indent1");
			if (summaryNav != null) WriteNodeFixCode(xtw, summaryNav, "p");
			if (remarksNav != null) WriteNodeFixCode(xtw, remarksNav, "p");
			MakeHtmlForMethodParameterDetails(nav, xtw);
			xtw.WriteEndElement(); // div
		}

		/// <summary>
		/// Write a parameter list to the specified writer.
		/// </summary>
		/// <param name="nav"><see cref="XPathNavigator" /> to the method's node.</param>
		/// <param name="xtw">The <see cref="XmlTextWriter" /> to write the parameter list to.</param>
		private void MakeHtmlForMethodParameterDetails(XPathNavigator nav, XmlTextWriter xtw)
		{
			// add params
			ArrayList parameterList = GetChildren(nav, "parameter");
			ArrayList paramList = GetDescendants(nav, "param");

			if (parameterList.Count > 0)
			{
				xtw.WriteStartElement("p");
				xtw.WriteElementString("lh", "Parameters:");
				foreach(XPathNavigator n3 in parameterList)
				{
					string name = n3.GetAttribute("name", "");
					string type = n3.GetAttribute("type", "");
					string simpleType = ToSimpleType(type);
					string inOutRef = n3.GetAttribute("huh?", "");

					StringBuilder sb = new StringBuilder();
					if (inOutRef.Length > 0) sb.Append(inOutRef + " ");
					sb.Append(simpleType + " " + name);

					// get description, which is the value of another node!
					string desc = string.Empty;
					foreach(XPathNavigator descNav in paramList)
					{
						string tmpName = descNav.GetAttribute("name", "");
						if (tmpName == name)
						{
							desc = descNav.Value;
							break;
						}
					}

					xtw.WriteRaw(String.Format("<li class=\"indent1\">{0} : {1}</li>", sb.ToString(), desc));
				}
				xtw.WriteEndElement();
			}
		}

		/// <summary>
		/// Make a string for a method (including constructor) declaration, such as 
		/// "public bool Foo(int x, int y, string s)".
		/// </summary>
		/// <param name="nav">Navigator to the Method's node.</param>
		/// <param name="parentTypeName">The name of the Type which contains this
		/// method.</param>
		/// <returns>The declaration string.</returns>
		string MakeMethodDeclaration(XPathNavigator nav, string parentTypeName)
		{
			string memberAccess = nav.GetAttribute("access", "");
			string memberName = nav.GetAttribute("name", "");
			string returnType = ToSimpleType(nav.GetAttribute("returnType", ""));

			string args = MakeMethodParametersString(nav);
			string nameString;
			if (memberName.Equals(".ctor"))
			{
				nameString = KeyWrap(ToAccessDeclaration(memberAccess) + " " 
					+ parentTypeName + "(" + args + ")");
			}
			else
			{
				nameString = KeyWrap(ToAccessDeclaration(memberAccess) + " " 
					+ TypeRefWrap(returnType) + " " 
					+ memberName + "(" + args + ")");
			}

			return(nameString);
		}

		/// <summary>
		/// Make a string for a method's parameter list, such as 
		/// "int x, int y, string s".
		/// </summary>
		/// <param name="nav">Navigator to the Method's node.</param>
		/// <returns>The param list string.</returns>
		string MakeMethodParametersString(XPathNavigator nav)
		{
			StringBuilder sb = new StringBuilder();

			// add params
			ArrayList paramList = GetChildren(nav, "parameter");
			if (paramList.Count > 0)
			{
				bool first = true;
				foreach(XPathNavigator n3 in paramList)
				{
					if (!first) sb.Append(", ");
					first = false;
					string name = n3.GetAttribute("name", "");
					string type = n3.GetAttribute("type", "");
					string simpleType = ToSimpleType(type);
					string inOutRef = n3.GetAttribute("huh?", "");
					if (inOutRef.Length > 0) sb.Append(inOutRef + " ");
					sb.Append(simpleType + " " + name);
				}
			}

			return(TypeRefWrap(sb.ToString()));
		}

		/// <summary>
		/// Make a string for a Type declaration, such as 
		/// "public class Foo : IComparable".
		/// </summary>
		/// <param name="nav">Navigator to the Type's node.</param>
		/// <returns>The declaration string.</returns>
		string MakeHtmlTypeDeclaration(XPathNavigator nav)
		{
			string nodeName = nav.GetAttribute("name", "");
			string nodeType = nav.LocalName;
			string typeAccess = nav.GetAttribute("access", "");
			string baseType = nav.GetAttribute("baseType", "");
			string abstractString = nav.GetAttribute("abstract", "");
			if (abstractString.Length == 0) abstractString = " ";
			else if (abstractString.Equals("true")) abstractString = " abstract ";

			// put in declaration
			string lcTypeAccess = ToAccessDeclaration(typeAccess);
			string declarationString = lcTypeAccess + abstractString + nodeType + " " + nodeName;
			if (baseType.Length > 0) declarationString += " : " + baseType;

			// add interfaces to declaration string
			ArrayList implKids = GetChildren(nav, "implements");
			if (implKids.Count > 0)
			{
				// add appropriate separator
				if (baseType.Length == 0) declarationString += " : ";
				else declarationString += ", ";

				bool first = true;
				foreach(XPathNavigator n3 in implKids)
				{
					if (!first) declarationString += ", ";
					first = false;
					declarationString += n3.GetAttribute("type", "");
				}
			}

			return(declarationString);
		}

		#endregion

		#endregion

		#region EmitHtml

		/// <summary>
		/// This writes the html corresponding to the xml we've already
		/// internalized.
		/// </summary>
		/// <param name="fileName">The name of the file to write to.</param>
		/// <returns></returns>
		private bool EmitHtml(string fileName)
		{
			StreamWriter sw = File.CreateText(fileName);
			Stream fs = sw.BaseStream;

			//
			// doc head
			//
			XmlTextWriter topWriter = new XmlTextWriter(fs, Encoding.UTF8);
			//topWriter.Formatting = Formatting.Indented;
			topWriter.Indentation = 4;
			//topWriter.WriteRaw("<html dir=\"LTR\">\n");
			topWriter.WriteStartElement("html");
			topWriter.WriteAttributeString("dir", "LTR");

			topWriter.WriteStartElement("head");

			topWriter.WriteElementString("title", "Example");
			topWriter.WriteRaw("	<META http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">\n");
			topWriter.WriteRaw("		<meta name=\"vs_targetSchema\" content=\"http://schemas.microsoft.com/intellisense/ie5\">\n	");
			//		<xml></xml><link rel="stylesheet" type="text/css" href="MSDN.css">
			topWriter.WriteRaw("		<LINK rel=\"stylesheet\" href=\"LinearHtml.css\" type=\"text/css\">\n");
			topWriter.WriteEndElement(); // head
			//topWriter.WriteRaw("	<body>\n");
			topWriter.WriteStartElement("body");
			topWriter.WriteString(" "); // to close previous start, because of interleaved writers to same stream
			topWriter.Flush();

			//
			// namespace list
			//
			MemoryStream ms = (MemoryStream)namespaceListWriter.BaseStream;
			ms.Position = 0;
			fs.Write(ms.GetBuffer(), 0, (int)ms.Length);
			fs.Flush();

			// namespace section header
//			topWriter.WriteElementString("h1", "Namespace Specifications");
//			topWriter.Flush();

			//
			// namespaces
			//
			XmlTextWriter xtw;

			// build list of sections for namespaces
			ArrayList nsSectionList = new ArrayList(orderedNamespaceSections);
			nsSectionList.Insert(0, "typeList"); // use this for the type list too

			foreach(string namespaceName in namespaceWriters.Keys)
			{
				topWriter.WriteElementString("h1", "Namespace : " + namespaceName);
				topWriter.Flush();
				Hashtable nsSectionWriters = (Hashtable)namespaceWriters[namespaceName];

				foreach(string sectionKey in nsSectionList)
				{
					string sectionName = (string)namespaceSections[sectionKey];

					xtw = null;
					if (nsSectionWriters.ContainsKey(sectionName))
					{
						// so something was written to this section
						xtw = (XmlTextWriter)nsSectionWriters[sectionName];
						xtw.Flush();

						// copy to output stream
						ms = (MemoryStream)xtw.BaseStream;
						ms.Position = 0;
						fs.Write(ms.GetBuffer(), 0, (int)ms.Length);
					}
					else
					{
						// nothing in this section
					}
				}
			}

			// doc close
			topWriter.WriteEndElement(); // body
			topWriter.WriteEndElement(); // html
			topWriter.Flush();

			fs.Close();
			return(true);
		}

		#endregion

		#region Main

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("Usage: <input file> <output file>");
				return;
			}

			string fileName = args[0];
			string outFileName = args[1];
			Console.WriteLine("Starting for file {0}, output {1}", fileName, outFileName);
			LinearHtmlDocumenterInfo info = new LinearHtmlDocumenterInfo();

			LinearHtmlDocumenter ld = new LinearHtmlDocumenter( new LinearHtmlDocumenterConfig( info ) );
			ld.Load(fileName);
			ld.MakeHtml(outFileName);
		}

		#endregion
	}
}
