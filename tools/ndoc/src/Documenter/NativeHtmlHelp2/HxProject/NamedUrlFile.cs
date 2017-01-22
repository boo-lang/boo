using System;
using System.Xml;

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{
	/// <summary>
	/// Class used for accessing the content of a named url index
	/// </summary>
	public class NamedUrlFile : IndexFile
	{
		/// <summary>
		/// Creates a new instance of a NamedUrlFile based on a templae
		/// </summary>
		/// <param name="templateFile">The path to the template</param>
		/// <param name="name">The name of the new file</param>
		/// <returns>A new NamedUrl file</returns>
		public static new NamedUrlFile CreateFrom( string templateFile, string name )
		{
			return new NamedUrlFile( name, HxFile.CreateFrom( templateFile ) );
		}

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="node"><see cref="System.Xml.XmlNode"/> representing the file data</param>		 
		public NamedUrlFile( string name, XmlNode node ) : base( name, node )
		{

		}

		/// <summary>
		/// HTML file that displays the Help About image.
		/// </summary>
		public string IntroductionPage
		{
			get{ return GetProperty( "Keyword[@Term='HomePage']/Jump/@Url" ); }
			set
			{
				SetProperty( "Keyword[@Term='HomePage']/Jump/@Url", value );
				SetProperty( "Keyword[@Term='DefaultPage']/Jump/@Url", value );
			}
		}

		/// <summary>
		/// Displays product information in Help About.
		/// </summary>
		public string AboutPageIcon
		{
			get{ return GetProperty( "Keyword[@Term='AboutPageIcon']/Jump/@Url" ); }
			set
			{
				SetProperty( "Keyword[@Term='AboutPageIcon']/Jump/@Url", value );
			}
		}

		/// <summary>
		/// The url used for both the DefaultPage and the HomePage named urls
		/// </summary>
		public string AboutPageInfo
		{
			get{ return GetProperty( "Keyword[@Term='AboutPageInfo']/Jump/@Url" ); }
			set
			{
				SetProperty( "Keyword[@Term='AboutPageInfo']/Jump/@Url", value );
			}
		}

		/// <summary>
		/// Displays when a user chooses a keyword index term that has subkeywords but is not directly associated with a topic itself.
		/// </summary>
		public string EmptyIndexTerm
		{
			get{ return GetProperty( "Keyword[@Term='EmptyIndexTerm']/Jump/@Url" ); }
			set
			{
				SetProperty( "Keyword[@Term='EmptyIndexTerm']/Jump/@Url", value );
			}
		}

		/// <summary>
		/// Opens if a link to a topic or URL is broken.
		/// </summary>
		public string NavFailPage
		{
			get{ return GetProperty( "Keyword[@Term='NavFailPage']/Jump/@Url" ); }
			set
			{
				SetProperty( "Keyword[@Term='NavFailPage']/Jump/@Url", value );
			}
		}
	}
}
