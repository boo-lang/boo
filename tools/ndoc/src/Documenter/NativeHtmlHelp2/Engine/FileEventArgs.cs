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

namespace NDoc.Documenter.NativeHtmlHelp2.Engine
{
	/// <summary>
	/// EventArgs calss used by the HtmlFactory calss when new files are being added
	/// </summary>
	public class FileEventArgs : EventArgs
	{

		/// <summary>
		/// The name of the file being added or inserted
		/// </summary>
		public readonly string File;

		/// <summary>
		/// Creates a new instnace of a FileEventArgs class
		/// </summary>
		/// <param name="file">The name of the file being added or inserted</param>
		public FileEventArgs( string file )
		{
			File = file;
		}
	}
}
