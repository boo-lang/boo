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
using System.Text;
using System.Diagnostics;

namespace NDoc.Documenter.NativeHtmlHelp2.Compiler
{
	/// <summary>
	/// Wraps the HxReg.exe help registry component
	/// </summary>
	public class HxReg : HxObject
	{
		/*
			Microsoft Help Help File Registration Tool Version 2.1.9466
			Copyright (c) 1999-2000 Microsoft Corp.

			Usage: HxReg [switches] | HxReg <Help filename .HxS>
				-n <namespace>
				-i <title ID>
				-c <collection Name .HxC | .HxS>
				-d <namespace description>
				-s <Help filename .HxS>
				-x <Help Index filename .HxI>
				-q <Help Collection Combined FTS filename .HxQ>
				-t <Help Collection Combined Attribute Index filename .HxR>
				-l <language ID>
				-a <alias>
				-f <filename listing HxReg commands>
				-r Remove a namespace, Help title, or alias

			EXAMPLES
			To register a namespace:
				HxReg -n <namespace> -c <collection filename> -d <namespace description>
			To register a Help file:
				HxReg -n <namespace> -i <title id> -s <HxS filename>		  
		*/

		/// <summary>
		/// Create a new instance of a HxReg object
		/// </summary>
		public HxReg() : base( "HxReg.exe" )
		{
		}

		/// <summary>
		/// Removes a help namespace from the help registry
		/// </summary>
		/// <param name="helpNamespace">The help namespace to remove</param>
		public void UnregisterNamespace( string helpNamespace )
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( " -n {0} ", helpNamespace );
			
			sb.Append( " -r " );

			Execute( sb.ToString(), "." );
			Trace.WriteLine( string.Format( "Unregistering {0} help namespace", helpNamespace ) );
		}

		/// <summary>
		/// Removes a help title from the help registry
		/// </summary>
		/// <param name="helpNamespace">The help namespace that contains the title</param>
		/// <param name="titleId">The id of the title to remove</param>
		public void UnregisterTitle( string helpNamespace, string titleId )
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( " -n {0} ", helpNamespace );
			sb.AppendFormat( " -i {0} ", titleId );
			
			sb.Append( " -r " );

			Execute( sb.ToString(), "." );
			Trace.WriteLine( string.Format( "Unregistering {0} help title", titleId ) );
		}
			

		/// <summary>
		/// Registers a help namespace
		/// </summary>
		/// <param name="helpNamespace">The ID of the namespace to register</param>
		/// <param name="collectionFile">The collection (HxS or HxC) file</param>
		/// <param name="description">Namespace description</param>
		public void RegisterNamespace( string helpNamespace, FileInfo collectionFile, string description )
		{
			Debug.Assert( collectionFile != null );
			Debug.Assert( collectionFile.Exists );

			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( " -n {0} ", helpNamespace );
			
			sb.Append( " -c " );
			sb.Append( '"' );
			sb.Append( collectionFile.FullName );
			sb.Append( '"' );

			sb.AppendFormat( " -d " );
			sb.Append( '"' );
			sb.AppendFormat( description );
			sb.Append( '"' );

			Execute( sb.ToString(), collectionFile.Directory.FullName );

			Trace.WriteLine( "Namespace registered successfully" );
		}

		/// <summary>
		/// Register an HxS title with a help namespace
		/// </summary>
		/// <param name="helpNamespace">The namespace to register with</param>
		/// <param name="titleID">The id of the new title</param>
		/// <param name="hxsFile">The location of the HxS file</param>
		public void RegisterTitle( string helpNamespace, string titleID, FileInfo hxsFile )
		{
			Debug.Assert( hxsFile != null );
			Debug.Assert( hxsFile.Exists );
 
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat( " -n {0} ", helpNamespace );
			sb.AppendFormat( " -i {0} ", titleID );

			sb.Append( '"' );
			sb.Append( hxsFile.FullName );
			sb.Append( '"' );

			Execute( sb.ToString(), hxsFile.Directory.FullName );
			Trace.WriteLine( "Title registered successfully" );
		}
	}
}
