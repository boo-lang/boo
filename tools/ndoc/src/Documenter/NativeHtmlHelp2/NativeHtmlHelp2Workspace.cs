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

using NDoc.Core;

namespace NDoc.Documenter.NativeHtmlHelp2
{
	/// <summary>
	/// Manages the location of the documentation build process
	/// </summary>
	public class NativeHtmlHelp2Workspace : Workspace
	{
		/// <summary>
		/// Contructs a new instance of the NativeHtmlHelp2Workspace class
		/// </summary>
		/// <param name="rootDir">The location to create the workspace</param>
		public NativeHtmlHelp2Workspace( string rootDir ) : base( rootDir, "htmlhelp2", ContentLocationName, "*.HxS;*.HxI;*.HxC;*.h2reg.ini;*.HxT;*.HxK" )
		{

		}

		/// <summary>
		/// The name of the relative location (from the porject root) where content files are stored
		/// </summary>
		public static string ContentLocationName
		{
			get
			{
				return "html";
			}
		}
	}
}
