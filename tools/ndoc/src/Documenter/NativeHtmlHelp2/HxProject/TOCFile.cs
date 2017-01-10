// MsdnDocumenter.cs - a MSDN-like documenter
// Copyright (C) 2003 Don Kackman
// Parts copyright 2001  Kral Ferch, Jason Diamond
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
using System.Xml;
using System.Text;
using System.Diagnostics;

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{

	/// <summary>
	/// Calls used for accessing a table of contents file
	/// </summary>
	public class TOCFile : HxFile
	{

		/// <summary>
		/// Creates a new instance of a TOCFile based on a templae
		/// </summary>
		/// <param name="templateFile">The path to the template</param>
		/// <param name="name">The name of the new file</param>
		/// <returns>A new toc file</returns>
		public static TOCFile CreateFrom( string templateFile, string name )
		{
			return new TOCFile( name, HxFile.CreateFrom( templateFile ) );
		}

		private XmlTextWriter xmlWriter = null;
		private StringWriter stringWriter = null;


		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="node"><see cref="System.Xml.XmlNode"/> representing the file data</param>
		protected TOCFile( string name, XmlNode node ) : base( name, node )
		{

		}

		/// <summary>
		/// <see cref="HxFile"/>.
		/// </summary>
		public override string FileName{ get{ return Name + ".HxT"; } }

		/// <summary>
		/// Opens the tables of contents for writing
		/// </summary>
		public void Open()
		{
			Debug.Assert( xmlWriter == null );
			stringWriter = new StringWriter();
			xmlWriter = new XmlTextWriter( stringWriter ) ;
			xmlWriter.QuoteChar = '\'';

			xmlWriter.WriteStartElement( "tmp" );
		}

		/// <summary>
		/// Starts a new node at the current context
		/// </summary>
		/// <param name="url">Url associated with the new node</param>
		public void OpenNode( string url )
		{
			xmlWriter.WriteStartElement( "HelpTOCNode" );
			xmlWriter.WriteAttributeString( "", "Url", "", url );
		}

		/// <summary>
		/// Inserts a new node into the current context without starting a new context
		/// </summary>
		/// <param name="url">Url associated with the new node</param>
		public void InsertNode( string url )
		{
			OpenNode( url );
			CloseNode();
		}

		/// <summary>
		/// Closes the current context
		/// </summary>
		public void CloseNode()
		{
			xmlWriter.WriteFullEndElement();
		}

		/// <summary>
		/// Closes the table of contents and saves the file
		/// </summary>
		public void Close()
		{
			Debug.Assert( xmlWriter != null );

			xmlWriter.WriteEndElement();
			xmlWriter.Close();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml( stringWriter.ToString() );

			foreach( XmlNode node in doc.DocumentElement.ChildNodes )			
				dataNode.AppendChild( dataNode.OwnerDocument.ImportNode( node, true ) );
			
			xmlWriter = null;
			stringWriter = null;
		}
	}
}
