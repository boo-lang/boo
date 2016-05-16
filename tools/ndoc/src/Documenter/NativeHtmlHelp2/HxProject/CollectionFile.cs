using System;
using System.Xml;

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{
	/// <summary>
	/// Summary description for CollectionFile.
	/// </summary>
	public class CollectionFile : ProjectFile
	{
		/// <summary>
		/// Creates a new instance of a CollectionFile based on a templae
		/// </summary>
		/// <param name="templateFile">The path to the template</param>
		/// <param name="name">The name of the new file</param>
		/// <returns>A new CollectionFile</returns>
		public static new CollectionFile CreateFrom( string templateFile, string name )
		{
			return new CollectionFile( name, HxFile.CreateFrom( templateFile ) );
		}

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="node"><see cref="System.Xml.XmlNode"/> representing the file data</param>
		protected CollectionFile( string name, XmlNode node ) : base( name, node )
		{

		}

		/// <summary>
		/// Adds a KeyWordIndexDef node to the collection file
		/// </summary>
		/// <param name="fileName">Name of the index def</param>
		public void AddKeywordIndex( string fileName )
		{
			XmlElement e = base.dataNode.OwnerDocument.CreateElement( "KeywordIndexDef" );
			e.SetAttribute( "File", fileName );
			XmlNode tocDef = base.dataNode.SelectSingleNode( "TOCDef" );

			base.dataNode.InsertAfter( e, tocDef );
		}
	}
}
