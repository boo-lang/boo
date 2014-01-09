using System;
using System.IO;
using System.Xml;

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{
	/// <summary>
	/// Class representing the Hx include file
	/// </summary>
	public class IncludeFile : HxFile
	{
		/// <summary>
		/// Creates a new instance of a IncludeFile based on a template
		/// </summary>
		/// <param name="templateFile">The path to the template</param>
		/// <param name="name">The name of the new file</param>
		/// <returns>A new IncludeFile file</returns>
		public static IncludeFile CreateFrom( string templateFile, string name )
		{
			return new IncludeFile( name, HxFile.CreateFrom( templateFile ) );
		}

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="node"><see cref="System.Xml.XmlNode"/> representing the file data</param>
		public IncludeFile( string name, XmlNode node ) : base( name, node )
		{

		}

		/// <summary>
		/// <see cref="HxFile"/>.
		/// </summary>
		public override string FileName{ get{ return Name + ".HxF"; } }

		/// <summary>
		/// Adds a directory to the include file (includes *.* in that directory)
		/// </summary>
		/// <param name="path">Relative path (from the include file) to the directory</param>
		public void AddDirectory( string path )
		{
			XmlElement fileNode = base.dataNode.OwnerDocument.CreateElement( "File" );
			fileNode.SetAttribute( "Url", Path.Combine( path, "*.*" ) );
			dataNode.AppendChild( fileNode );
		}
	}
}
