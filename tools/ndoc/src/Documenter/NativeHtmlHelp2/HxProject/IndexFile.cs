using System;
using System.Xml;

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{
	/// <summary>
	/// Summary description for IndexFile.
	/// </summary>
	public class IndexFile : HxFile
	{
		/// <summary>
		/// Constructs a new instance of the IndexFile class
		/// from the specified template
		/// </summary>
		/// <param name="templateFile">Path to the file template</param>
		/// <param name="name">The name of the new file</param>
		/// <returns>The new IndexFile</returns>
		public static IndexFile CreateFrom( string templateFile, string name )
		{
			return new IndexFile( name, HxFile.CreateFrom( templateFile ) );
		}

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="node"><see cref="System.Xml.XmlNode"/> representing the file data</param>
		protected IndexFile( string name, XmlNode node ) : base( name, node )
		{

		}

		/// <summary>
		/// <see cref="HxFile"/>.
		/// </summary>
		public override string FileName{ get{ return Name + ".HxK"; } }

	}
}
