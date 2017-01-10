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

namespace NDoc.Documenter.NativeHtmlHelp2.HxProject
{
	/// <summary>
	/// Represents the contents of an Html Help 2 HxC project file
	/// </summary>
	public class ProjectFile : HxFile
	{
		/// <summary>
		/// Creates a new instance of a ProjectFile based on a templae
		/// </summary>
		/// <param name="templateFile">The path to the template</param>
		/// <param name="name">The name of the new file</param>
		/// <returns>A new ProjectFile</returns>
		public static ProjectFile CreateFrom( string templateFile, string name )
		{
			return new ProjectFile( name, HxFile.CreateFrom( templateFile ) );
		}

		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		/// <param name="name">The name of the file</param>
		/// <param name="node"><see cref="System.Xml.XmlNode"/> representing the file data</param>
		protected ProjectFile( string name, XmlNode node ) : base( name, node )
		{

		}

		/// <summary>
		/// <see cref="HxFile"/>.
		/// </summary>
		public override  string FileName{ get{ return Name + ".HxC"; } }

		/// <summary>
		/// The project title
		/// </summary>
		public string Title
		{
			get{ return GetProperty( "@Title" ); }
			set{ SetProperty( "@Title", value ); }
		}
		/// <summary>
		/// Copyright text for the project
		/// </summary>
		public string Copyright
		{
			get{ return GetProperty( "@Copyright" ); }
			set{ SetProperty( "@Copyright", value ); }
		}
		/// <summary>
		/// Help project version
		/// </summary>
		public string FileVersion
		{
			get{ return GetProperty( "@FileVersion" ); }
			set{ SetProperty( "@FileVersion", value ); }
		}
		/// <summary>
		/// When true builds both an HxI and an HxS file. Otherwise the index is compiled into the HxS
		/// </summary>
		public bool BuildSeparateIndexFile
		{
			get{ return GetProperty( "CompilerOptions/@CompileResult" ) == "HxiHxs"; }
			set{ SetProperty( "CompilerOptions/@CompileResult", value ? "HxiHxs" : "Hxs" ); }
		}
		/// <summary>
		/// Indicates whether to creare a full text index while compiling the porject
		/// </summary>
		public bool CreateFullTextIndex
		{
			get{ return GetProperty( "CompilerOptions/@CreateFullTextIndex" ) == "Yes"; }
			set{ SetProperty( "CompilerOptions/@CreateFullTextIndex", value ? "Yes" : "No" ); }
		}
		/// <summary>
		/// The name of the stop word list to include in the project. A stop word list
		/// is a list of common words that will be ignored during full text searching
		/// </summary>
		public string StopWordFile
		{
			get{ return GetProperty( "CompilerOptions/@StopWordFile" ); }
			set{ SetProperty( "CompilerOptions/@StopWordFile", value ); }
		}
		/// <summary>
		/// The name of the table fo contents file for the project
		/// </summary>
		public string TOCFile
		{
			get{ return GetProperty( "TOCDef/@File" ); }
			set{ SetProperty( "TOCDef/@File", value ); }
		}
	}
}
