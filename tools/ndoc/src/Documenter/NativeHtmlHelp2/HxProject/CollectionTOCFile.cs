using System;
using System.Xml;

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{
	/// <summary>
	/// Summary description for CollectionTOCFile.
	/// </summary>
	public class CollectionTOCFile : HxFile
	{

		/// <summary>
		/// Constructs a new instance of the CollectionTOCFile class
		/// from the specified template
		/// </summary>
		/// <param name="templateFile">Path to the file template</param>
		/// <param name="name">The name of the new file</param>
		/// <returns>The new CollectionTOCFile</returns>
		public static CollectionTOCFile CreateFrom( string templateFile, string name )
		{
			return new CollectionTOCFile( name, HxFile.CreateFrom( templateFile ) );
		}

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="node"><see cref="System.Xml.XmlNode"/> representing the file data</param>
		protected CollectionTOCFile( string name, XmlNode node ) : base( name, node )
		{

		}

		/// <summary>
		/// <see cref="HxFile"/>.
		/// </summary>
		public override string FileName{ get{ return Name + ".HxT"; } }

		/// <summary>
		/// The title displayed in the help browser
		/// (Ignored if <see cref="Flat"/> is true
		/// </summary>
		public string Title
		{
			get{ return GetProperty( "@PluginTitle" ); }
			set{ SetProperty( "@PluginTitle", value ); }
		}

		/// <summary>
		/// Will the collection TOC be falt in the help browser.
		/// If false the TOC will be hierarchical
		/// </summary>
		public bool Flat
		{
			get{ return GetProperty( "@PluginStyle" ) == "Flat"; }
			set{ SetProperty( "@PluginStyle", value ? "Flat" : "Hierarchical" ); }
		}

		/// <summary>
		/// The Url of the title that will be displayed by this TOC
		/// </summary>
		public string BaseUrl
		{
			get{ return GetProperty( "HelpTOCNode/@Url" ); }
			set{ SetProperty( "HelpTOCNode/@Url", value ); }
		}

	}
}
