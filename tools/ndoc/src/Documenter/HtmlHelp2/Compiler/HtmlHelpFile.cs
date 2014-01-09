using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;

using mshtml;

namespace NDoc.Documenter.HtmlHelp2.Compiler
{
	/// <summary>
	/// Describes what type of thing the help file describes
	/// </summary>
	internal enum HelpTopicType
	{
		Class,
		Interface,
		Structure,
		Field,
		Enumeration,
		Delegate,
		MemberList,
		Constructor,
		Method,
		Property,
		Namespace,
		Unknown
	};

	/// <summary>
	/// Summary description for HtmlHelpFile.
	/// </summary>
	internal class HtmlHelpFile
	{
		private HTMLDocumentClass m_doc = null;
		IHTMLElement m_dataIsland = null;
		private FileInfo m_file = null;
		private bool m_dirty = false;
		private HelpTopicType m_TopicType = HelpTopicType.Unknown;

		/// <summary>
		/// Creates a new instance of the HtmlHelpFile class
		/// </summary>
		/// <param name="f">The path to the file</param>
		public HtmlHelpFile( FileInfo f )
		{
			try 
			{
				Debug.Assert( f != null );
				Debug.Assert( f.Exists );

				m_file = f;
				m_doc = GetHtmlDocument( f );
				m_dataIsland = GetXmlDataIsland();
				m_TopicType = GetTopicType();
			}
			catch ( Exception e )
			{
				throw new ArgumentException( string.Format( "Could not open {0}", f.Name ), "f", e );
			}
		}

		/// <summary>
		/// The name of the type (class, member, namespace) that the file describes
		/// </summary>
		public string TypeName
		{
			get
			{
				HelpTopicType type = TopicType;

				if ( type == HelpTopicType.Property || type == HelpTopicType.Method )
				{
					// for properties and methods the type is the last word (before the extension)
					string fileName = m_file.Name;

					int lastDot = fileName.LastIndexOf( '.' );
					int penultimateDot = fileName.LastIndexOf( '.', lastDot - 1 );

					return fileName.Substring( penultimateDot + 1, lastDot - penultimateDot - 1 );
				}
				else if ( type == HelpTopicType.Namespace )
				{
					// for namespaces it's just the html title
					return Title;
				}
				else
				{
					// otherwise just get the leftmost word of the doc title
					return Title.Substring( 0, Title.IndexOf( ' ' ) );
				}
			}
		}

		/// <summary>
		/// The namespace to which the described type belongs
		/// </summary>
		public string ParentNamespace
		{
			get
			{
				string fileName = m_file.Name;

				int lastDot = fileName.LastIndexOf( '.' );
				int penultimateDot = fileName.LastIndexOf( '.', lastDot - 1 );

				return fileName.Substring( 0, penultimateDot );
			}
		}
		
		/// <summary>
		/// The type of thing that the file describes
		/// </summary>
		public HelpTopicType TopicType
		{
			get
			{
				return m_TopicType;
			}
		}

		/// <summary>
		/// persists the Html document back to its original location
		/// </summary>
		public void Save()
		{			
			using( StreamWriter s = new StreamWriter( m_file.OpenWrite() ) )
			{
				s.Write( m_doc.documentElement.outerHTML );
			}
		}

		/// <summary>
		/// Describes whether the document has beeen changed since it was opened
		/// </summary>
		public bool Dirty
		{
			get
			{
				return m_dirty;
			}
		}

		/// <summary>
		/// The title of the document
		/// </summary>
		public string Title
		{
			get
			{
				return m_doc.title;
			}
		}

		/// <summary>
		/// Return the title used in the First H1 tag
		/// </summary>
		public string HeadingTitle
		{
			get
			{
				string ret = string.Empty;

				IEnumerator headings = m_doc.getElementsByTagName( "H1" ).GetEnumerator();
				
				if ( headings.MoveNext() )
				{
					try
					{
						ret = ((IHTMLElement)headings.Current).innerText;
					}
					catch(InvalidCastException)
					{
						ret = string.Empty;
					}
				}

				return ret;
			}
		}

		/// <summary>
		/// The Xml data island in this html help file
		/// (if there is more than one returns the last one , in document order
		/// </summary>
		/// <returns>IHTMLElement pointer</returns>
		private IHTMLElement GetXmlDataIsland()
		{
			IHTMLElement dataIsland = null;

			IEnumerator islands = m_doc.getElementsByTagName( "xml" ).GetEnumerator();

			//spin through the xml data islands until the last one and return it
			while ( islands.MoveNext() )
				dataIsland = (IHTMLElement)islands.Current;

			Trace.WriteLineIf( dataIsland == null, string.Format( "No Xml data island found in file {0}", m_doc.URLUnencoded ) );

			return dataIsland;
		}

		/// <summary>
		/// Sets the contents of the Xml data island (if present)
		/// </summary>
		/// <param name="html">The new html</param>
		public void AppendDataIslandHtml( StringBuilder html )
		{
			if ( m_dataIsland != null && html.Length > 0 )
			{					
				// inserAdjacentHTML seems to be replacing the existing information in the data island
				// so we are going to just append any exisiting HTML to our new Html and set the innerHTML property
				html.Append( m_dataIsland.innerHTML );
				
				m_dataIsland.innerHTML = html.ToString();
				//m_dataIsland.insertAdjacentHTML( "AfterBegin", html );
				m_dirty = true;
			}
		}

		/// <summary>
		/// Opens the HTML document at the specified location
		/// </summary>
		/// <param name="f">The Html file</param>
		/// <returns>Pointer to the parsed Html document</returns>
		private HTMLDocumentClass GetHtmlDocument( FileInfo f )
		{
			HTMLDocumentClass doc = null;
			
			try
			{
				doc = new HTMLDocumentClass();

				UCOMIPersistFile persistFile = (UCOMIPersistFile)doc;
				persistFile.Load( f.FullName, 0 );

				int start = Environment.TickCount;

				while( doc.body == null ) 
				{ 
					// as precaution to ensure that the html is fully parsed
					// we spin here (for a maximum of 10 seconds) until the 
					// body property is non-null
					if ( Environment.TickCount - start > 10000 )
					{
						Trace.WriteLine( string.Format( "The document {0} timed out while loading", f.Name ) );
						throw new Exception( string.Format( "The document {0} timed out while loading", f.Name ) );
					}
				}
			}
			catch( Exception e )
			{
				Trace.WriteLine( string.Format( "An error occured opening file {0}, {1}", f.Name, e.Message ) );
				throw e;
			}

			return doc;
		}

		private HelpTopicType GetTopicType()
		{
			HelpTopicType ret = HelpTopicType.Unknown;

			string title = Title.ToLower();

			// here we are using the title of the html file to try an intuit what
			// type of topic it is about
			// because by this point we have lost all of the contextual information about the real type we are
			// documenting 
			if ( title.EndsWith( " constructor" ) )
				ret = HelpTopicType.Constructor;
			else if ( title.EndsWith( " method" ) )
				ret = HelpTopicType.Method;
			else if ( title.EndsWith( " property" ) )
				ret = HelpTopicType.Property;
			else if ( title.EndsWith( " delegate" ) )
				ret = HelpTopicType.Delegate;
			else if ( title.EndsWith( " structure" ) )
				ret = HelpTopicType.Structure;
			else if ( title.EndsWith( " class" ) )
				ret = HelpTopicType.Class;
			else if ( title.EndsWith( " interface" ) )
				ret = HelpTopicType.Interface;
			else if ( title.EndsWith( " enumeration" ) )
				ret = HelpTopicType.Enumeration;
			else if ( title.EndsWith( " members" ) )
				ret = HelpTopicType.MemberList;
			else if ( title.EndsWith( " field" ) )
				ret = HelpTopicType.Field;
			//namesapce files don't have any indication in the title
			//so we're gonna peek inside the document a bit more
			else if ( HeadingTitle.ToLower().EndsWith( " namespace" ) )
				ret = HelpTopicType.Namespace;

			return ret;
		}

	}
}