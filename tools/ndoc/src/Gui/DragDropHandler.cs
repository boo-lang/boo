using System;
using System.IO;
using System.Collections;

using NDoc.Core;

namespace NDoc.Gui
{
	/// <summary>
	/// The type of files that can be dropped
	/// </summary>
	public enum DropFileType
	{
		/// <summary>
		/// One or more assemblies (dll or exe)
		/// </summary>
		Assembly,
		/// <summary>
		/// An .ndoc project file
		/// </summary>
		Project,
		/// <summary>
		/// Any other file type
		/// </summary>
		Unsupported
	}

	/// <summary>
	/// Handles drap and drop operations
	/// </summary>
	public sealed class DragDropHandler
	{
		private DragDropHandler()
		{
		}

		/// <summary>
		/// Determines if the list of files is a list of assemblies
		/// </summary>
		/// <param name="files">File list</param>
		/// <returns>True if all the files are dll's or exe's</returns>
		public static DropFileType CanDrop( string[] files )
		{
			if ( files.Length > 0 )
			{
				// the first item is an ndoc file so return the project type
				if ( Path.GetExtension( files[0] ).ToLower() == ".ndoc" )
					return DropFileType.Project;
				
				// otherwise we're going to look for assembly extensions
				foreach (string s in files)
				{
					string ext = Path.GetExtension( s ).ToLower();
					// this file isn't an assembly so it is unsupported
					if ( ext != ".dll" && ext != ".exe" )
						return DropFileType.Unsupported;
				}
				return DropFileType.Assembly;
			}

			return DropFileType.Unsupported;
		}

		/// <summary>
		/// Create a collection of <see cref="AssemblySlashDoc"/> objects
		/// </summary>
		/// <param name="files">An arrray of assembly files names</param>
		/// <returns>Populated collection</returns>
		public static ICollection GetAssemblySlashDocs( string[] files )
		{
			ArrayList assemblySlashDocs = new ArrayList();

			foreach (string s in files)
			{
				string ext = Path.GetExtension( s ).ToLower();
				if ( ext == ".dll" || ext == ".exe" )
				{
					string slashDocFile = FindDocFile( s );
					if ( slashDocFile.Length > 0 )
						assemblySlashDocs.Add( new AssemblySlashDoc( s, slashDocFile ) );
				}
			}
					
			return assemblySlashDocs;
		}

		/// <summary>
		/// Gets the path to a NDoc file project from the dropped files list
		/// </summary>
		/// <param name="files">The files dropped</param>
		/// <returns>The path stored in the first location or the files array</returns>
		public static string GetProjectFilePath( string[] files )
		{
			string path = string.Empty;
			if ( files.Length > 0 )
				path = files[0];
			return path;
		}

		private static string FindDocFile( string assemblyFile )
		{
			string slashDocFilename = assemblyFile.Substring( 0, assemblyFile.Length - 4 ) + ".xml";

			if ( File.Exists( slashDocFilename ) )
				return slashDocFilename;

			return "";
		}
	}
}
