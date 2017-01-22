using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;

using NDoc.Core;

namespace NDoc.Documenter.HtmlHelp2.Compiler
{


	/// <summary>
	/// Augments the Xml dat islands that HxConv creates by default
	/// in the html help files with additional index and MSHelp tags
	/// to increase integration with VS.NET
	/// </summary>
	public class HxAugmenter
	{
		/// <summary>
		/// Augments all of the html files in the specified directory
		/// with additonal MSHelp tags
		/// </summary>
		/// <param name="workingDir">The root directory of the project</param>
		/// <param name="helpName">The name of the help project</param>
		static public void Augment( DirectoryInfo workingDir, string helpName )
		{
			Trace.WriteLine( "Adding F Index File to project" );

			// create our two new index files
			string FIndexName = CreateIndexFile( workingDir, "_F.HxK", helpName, "_FLinks" );
			//string AIndexName = CreateIndexFile( workingDir, "_A.HxK", helpName, "_ALinks" );

			//now add their names to the project include file
			string IncludeFilePath = Path.Combine( workingDir.FullName, helpName + ".HxF" );
			XmlDocument IncludeDoc = OpenDocument( IncludeFilePath );

			AddFileEntryToIncludeFile( IncludeDoc, FIndexName );
			//AddFileEntryToIncludeFile( IncludeDoc, AIndexName );

			//save the include fule
			IncludeDoc.Save( IncludeFilePath );

			// now add our indices to the project file
			string ProjectFilePath = Path.Combine( workingDir.FullName, helpName + ".HxC" );
			XmlDocument ProjectDoc = OpenDocument( ProjectFilePath );

			// add a KeywordIndexDef node for each our new indices
			AddKeywordIndexDefToProjectFile( ProjectDoc, FIndexName );
			//AddKeywordIndexDefToProjectFile( ProjectDoc, AIndexName );

			// now add the ItemMoniker for the Associative Index
			// ( HxConv creates an ItemMoniker for the F index even though it doesn't create the FIndex file )
			//			XmlElement ItemMonikerElement = ProjectDoc.CreateElement( "ItemMoniker" );
			//			ItemMonikerElement.SetAttribute( "Name", "!DefaultAssociativeIndex" );
			//			ItemMonikerElement.SetAttribute( "ProgId", "HxDs.HxIndex" );
			//			ItemMonikerElement.SetAttribute( "InitData", "A" );
			//
			//			XmlNode sibling = ProjectDoc.DocumentElement.SelectSingleNode( "ItemMoniker" );
			//			ProjectDoc.DocumentElement.InsertAfter( ItemMonikerElement, sibling );

			// save the project file
			ProjectDoc.Save( ProjectFilePath );

			// and lastly fix up all of the html files
			FixUp( workingDir );
		}

		/// <summary>
		/// Extracts the specified index resource from the assembly and adds it
		/// to the project directory
		/// </summary>
		/// <param name="workingDir">The project directory</param>
		/// <param name="resourceName">The name of the embedded resource</param>
		/// <param name="helpName">The name of the help project</param>
		/// <param name="IndexID">The index to assign to the new index</param>
		/// <returns>The file name (without path) of the new index file</returns>
		static private string CreateIndexFile( DirectoryInfo workingDir, string resourceName, string helpName, string IndexID )
		{
			string IndexName = helpName + resourceName;

			EmbeddedResources.WriteEmbeddedResource(
				typeof( HxAugmenter ).Module.Assembly,
				"NDoc.Documenter.HtmlHelp2.xml." + resourceName,
				workingDir.FullName,
				IndexName );

			string IndexPath = Path.Combine( workingDir.FullName, IndexName );

			XmlDocument doc = OpenDocument( IndexPath );

			doc.DocumentElement.SetAttribute( "Id", helpName + IndexID );

			doc.Save( IndexPath );

			return IndexName;
		}

		static private XmlDocument OpenDocument( string IncludeFilePath ) 
		{
			XmlValidatingReader reader = new XmlValidatingReader( new XmlTextReader( IncludeFilePath ) );
			reader.ValidationType = ValidationType.None;
			reader.XmlResolver = null;		

			XmlDocument doc = new XmlDocument();
			doc.Load( reader );
			reader.Close();			//make sure we close the reader before saving

			return doc;
		}

		static private void AddFileEntryToIncludeFile( XmlDocument includeDoc, string entryName )
		{
			XmlElement FileNode = includeDoc.CreateElement( "File" );
			FileNode.SetAttribute( "Url", entryName );
			includeDoc.DocumentElement.AppendChild( FileNode );			
		}

		static private void AddKeywordIndexDefToProjectFile( XmlDocument ProjectDoc, string IndexName )
		{
			Debug.Assert( ProjectDoc.DocumentElement != null );

			// per the DTD all of the KeywordIndexDef nodes need to be grouped together
			XmlNode sibling = ProjectDoc.DocumentElement.SelectSingleNode( "KeywordIndexDef" );
			Debug.Assert( sibling != null );

			XmlElement KeywordIndexDef = ProjectDoc.CreateElement( "KeywordIndexDef" );
			KeywordIndexDef.SetAttribute( "File", IndexName );

			ProjectDoc.DocumentElement.InsertAfter( KeywordIndexDef, sibling );
		}

		/// <summary>
		/// Iterates over all html files in the specified directory,
		/// adding additional tags to their XML data islands
		/// </summary>
		/// <param name="workingDir">The location of the html file to fixed</param>
		static public void FixUp( DirectoryInfo workingDir )
		{
			foreach( FileInfo f in workingDir.GetFiles( "*.html" ) )
			{
				FixUp( f );
			}
		}

		static private void FixUp( FileInfo f )
		{
			Trace.WriteLine( string.Format( "Adding additional xml nodes to {0}", f.Name ) );

			try 
			{
				HtmlHelpFile helpFile = new HtmlHelpFile( f );

				StringBuilder sb = new StringBuilder();
				
				GetStandardMsHelpText( sb, helpFile.Title );
				GetIndexText( sb, helpFile );
				
				helpFile.AppendDataIslandHtml( sb );

				if ( helpFile.Dirty )
					helpFile.Save();	
			}
			catch (Exception)
			{
				Trace.WriteLine( string.Format( "Error agumenting {0}, continuing anyway", f.Name ) );
			}
		}

		static private void GetIndexText( StringBuilder sb, HtmlHelpFile helpFile )
		{
			Debug.Assert( sb != null );

			switch ( helpFile.TopicType )
			{
				case HelpTopicType.Class :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Interface :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Structure :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Namespace :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}'/>", helpFile.TypeName );
					break;
				case HelpTopicType.Delegate :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Constructor :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.New'/>", helpFile.TypeName );
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}.New'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Method :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Property :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Enumeration :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.Field :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}'/>", helpFile.TypeName );
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
				case HelpTopicType.MemberList :
					sb.AppendFormat( "<MSHelp:Keyword Index='F' Term='{0}.{1}'/>", helpFile.ParentNamespace, helpFile.TypeName );
					break;
			}			
		}

		static private void GetStandardMsHelpText( StringBuilder sb, string title )
		{
			Debug.Assert( sb != null );

			sb.AppendFormat( "<MSHelp:TOCTitle Title='{0}'/>", title );
			sb.AppendFormat( "<MSHelp:RLTitle Title='{0}'/>", title  );

			//sb.Append( "<MSHelp:Attr Name='DocSet' Value='NETFramework'/>" );
			sb.Append( "<MSHelp:Attr Name='TopicType' Value='kbSyntax'/>" );
			sb.Append( "<MSHelp:Attr Name='DevLang' Value='CSharp'/>" );
			sb.Append( "<MSHelp:Attr Name='DevLang' Value='VB'/>" );
			sb.Append( "<MSHelp:Attr Name='DevLang' Value='C++'/>" );
			sb.Append( "<MSHelp:Attr Name='DevLang' Value='JScript'/>" );
			sb.Append( "<MSHelp:Attr Name='Technology' Value='WFC'/>" );
			sb.Append( "<MSHelp:Attr Name='Technology' Value='ManagedC'/>" );
			sb.Append( "<MSHelp:Attr Name='TechnologyVers' Value='kbWFC'/>" );
			sb.Append( "<MSHelp:Attr Name='TechnologyVers' Value='kbManagedC'/>" );
			sb.Append( "<MSHelp:Attr Name='Locale' Value='kbEnglish'/>" );
			sb.Append( "<MSHelp:Attr Name='HelpPriority' Value='1'/>" );
		}
	}
}
