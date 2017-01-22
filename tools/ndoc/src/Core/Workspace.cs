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
using System.Diagnostics;

namespace NDoc.Core
{
	/// <summary>
	/// Handler for content events
	/// </summary>
	public delegate void ContentEventHandler( string path );

	/// <summary>
	/// The Workspace class manages the Output directory and its subfolders
	/// where help file content and project files are used to compile the 
	/// final help collection
	/// </summary>
	public class Workspace
	{
		/// <summary>
		/// Event raised when a content directory is added
		/// </summary>
		public event ContentEventHandler ContentDirectoryAdded;

		/// <summary>
		/// Event raised when a content file is added
		/// </summary>
		public event ContentEventHandler ContentFileAdded;
		
		/// <summary>
		/// The name of the directory where the html file are created
		/// </summary>
		private string contentDir = "content";

		/// <summary>
		/// The location of the workspace and files
		/// </summary>
		private string rootDir = string.Empty;

		/// <summary>
		/// These are the output file type extensions that will be cleaned
		/// </summary>
		private string cleanableFileTypes = string.Empty;
		
		private string buildDir;

		/// <summary>
		/// Creates a new instance of the <see cref="Workspace"/> class.
		/// </summary>
		/// <param name="root">The location to create the workspace</param>
		/// <param name="type">The type of workspace</param>
		/// <param name="contentDirName">The name of the sub folder where content will be placed</param>
		/// <param name="cleanableExtensions">A semi-colon delimited list of file extensions that can be deleted when cleaning
		/// the root folder of the workspace (*.ex1;*.ex2)</param>
		public Workspace( string root, string type, string contentDirName, string cleanableExtensions )
		{
			if ( !Path.IsPathRooted( root ) )
				throw new ArgumentException( "A relative path cannot be used for a worksapce" );
			
			if ( type.Length == 0 )
				throw new ArgumentException( "The workspace type cannot be zero length", type );

			rootDir = root;

			buildDir = string.Format( "ndoc_{0}_temp", type );
			contentDir = contentDirName;
			cleanableFileTypes = cleanableExtensions;

			if ( !Directory.Exists( RootDirectory ) )
				Directory.CreateDirectory( RootDirectory );
		}

		/// <summary>
		/// The name of the directory where the compilation takes place
		/// </summary>
		public string WorkingDirectoryName
		{
			get
			{
				return buildDir;
			}
		}

		/// <summary>
		/// The name of the content directory
		/// </summary>
		public string ContentDirectoryName
		{
			get
			{
				return contentDir;
			}
		}

		/// <summary>
		/// The full path to the worksapce root.
		/// This is where project outputs will be saved when compilation
		/// is complete
		/// </summary>
		public string RootDirectory
		{
			get
			{
				return rootDir;
			}
		}

		/// <summary>
		/// Prepares the workspace, by creating working and content directories
		/// </summary>
		public void Prepare()
		{
			if ( !Directory.Exists( this.WorkingDirectory ) )
				Directory.CreateDirectory( this.WorkingDirectory );
			
			if ( !Directory.Exists( this.ContentDirectory ) )
				Directory.CreateDirectory( this.ContentDirectory );

			if ( !Directory.Exists( this.ResourceDirectory ) )
				Directory.CreateDirectory( this.ResourceDirectory );
		}

		/// <summary>
		/// The full path of the help content files
		/// </summary>
		public string ContentDirectory
		{
			get 
			{
				return Path.Combine( WorkingDirectory, ContentDirectoryName );
			}
		}

		/// <summary>
		/// The the full path to the directory where the compilation will run
		/// </summary>
		public string WorkingDirectory
		{
			get
			{
				return Path.Combine( RootDirectory, buildDir );
			}
		}

		/// <summary>
		/// The location where the xslt stylesheets will be unpacked
		/// </summary>
		public string ResourceDirectory
		{
			get
			{
				return Path.Combine( WorkingDirectory, "ndoc_resources" );
			}
		}

		/// <summary>
		/// Adds a sub directory to the resource directory
		/// </summary>
		/// <param name="dirName">The name of the sub directory</param>
		public void AddResourceDirectory( string dirName )
		{
			if ( !Directory.Exists( Path.Combine( ResourceDirectory, dirName ) ) )
				Directory.CreateDirectory( Path.Combine( ResourceDirectory, dirName ) );
		}

		/// <summary>
		/// Recursively deletes the ResourceDirectory
		/// </summary>
		public void RemoveResourceDirectory()
		{
			if ( Directory.Exists( ResourceDirectory ) )
				Directory.Delete( ResourceDirectory, true );
		}

		/// <summary>
		/// Saves files mathing the specified filter from the build directory to the root directory
		/// </summary>
		/// <param name="filter">File filter to search for</param>
		public void SaveOutputs( string filter )
		{
			DirectoryInfo dir = new DirectoryInfo( WorkingDirectory );
			foreach ( FileInfo f in dir.GetFiles( filter ) )
			{
				string newFile = Path.Combine( this.RootDirectory, f.Name );
				if ( File.Exists( newFile ) )
					File.Delete( newFile );
				f.MoveTo( newFile );
			}
		}

		/// <summary>
		/// Copies project resources into the workspace.
		/// Project files are files needed to compile the help file, but
		/// are not directly part of its content
		/// </summary>
		/// <param name="sourceDirectory">The path to the resources</param>
		public void ImportProjectFiles( string sourceDirectory )
		{
			ImportProjectFiles( sourceDirectory, "*.*" );
		}

		/// <summary>
		/// Copies project resources into the workspace
		/// Project files are files needed to compile the help file, but
		/// are not directly part of its content
		/// </summary>
		/// <param name="sourceDirectory">The path to the resources</param>
		/// <param name="filter">File filter to use when selecting files to import</param>
		public void ImportProjectFiles( string sourceDirectory, string filter )
		{
			if ( !Directory.Exists( sourceDirectory ) )
				throw new ArgumentException( string.Format( "The source location {0} does not exist", sourceDirectory ) );

			DirectoryInfo dir = new DirectoryInfo( sourceDirectory );
			foreach( FileInfo file in dir.GetFiles( filter ) )
				Workspace.ImportFile( file, WorkingDirectory );
		}

		private static void ImportFile( FileInfo file, string targetDir )
		{
			Debug.Assert( file != null );

			string targetName = Path.Combine( targetDir, file.Name );
			file.CopyTo( targetName, true );
			// clear any possible readonly flag so we can delete this file later during clean-up
			File.SetAttributes( targetName, FileAttributes.Normal );
		}

		/// <summary>
		/// Recursively copies the contents of sourceDirectory into the workspace content,
		/// maintainng the same directory structure
		/// </summary>
		/// <param name="sourceDirectory">The directory to import</param>
		public void ImportContentDirectory( string sourceDirectory )
		{
			if ( !Directory.Exists( sourceDirectory ) )
				throw new ArgumentException( string.Format( "The source location {0} does not exist", sourceDirectory ) );

			DirectoryInfo sourceDir = new DirectoryInfo( sourceDirectory );

			// first import all the files in the directory directly to content
			foreach( FileInfo file in sourceDir.GetFiles() )
				Workspace.ImportFile( file, this.ContentDirectory );

			// then recursively import any foldes in sourceDirectory
			foreach( DirectoryInfo dir in sourceDir.GetDirectories() )
				ImportDirectory( dir, new DirectoryInfo( this.ContentDirectory ) );
		}

		/// <summary>
		/// Raises the <see cref="Workspace.ContentDirectoryAdded"/> event
		/// </summary>
		/// <param name="relativePath">Path relative to the workspace root</param>
		protected virtual void OnContentDirectoryAdded( string relativePath )
		{
			if ( ContentDirectoryAdded != null )
				ContentDirectoryAdded( relativePath );
		}

		private void ImportDirectory( DirectoryInfo sourceDir, DirectoryInfo targetParent )
		{
			DirectoryInfo targetDir = new DirectoryInfo( Path.Combine( targetParent.FullName, sourceDir.Name ) );;

			if ( !targetDir.Exists )
				targetDir.Create();

			OnContentDirectoryAdded( GetRelativePath( new DirectoryInfo( this.WorkingDirectory ), targetDir ) );

			foreach( FileInfo file in sourceDir.GetFiles() )
				Workspace.ImportFile( file, targetDir.FullName );

			foreach( DirectoryInfo childDir in sourceDir.GetDirectories() )
				ImportDirectory( childDir, targetDir );
		}

		/// <summary>
		/// Return the relative path between two directories
		/// </summary>
		/// <param name="ancestor">The folder closest to the drive root</param>
		/// <param name="child">A folder that is a child of ancestor</param>
		/// <returns></returns>
		public static string GetRelativePath( DirectoryInfo ancestor, DirectoryInfo child )
		{	
			if ( ancestor.Root.FullName != child.Root.FullName )
				return "";

			string tmp = child.FullName.Replace( ancestor.FullName, "" );

			// strip off the leading backslash if present
			if ( tmp.IndexOf( Path.DirectorySeparatorChar, 0 ) == 0 )
				tmp = tmp.Substring( 1 );

			return tmp;
		}

		/// <summary>
		/// Copies content into the workspace ContentDirectory
		/// </summary>
		/// <param name="sourceDirectory">The path to the content files</param>
		public void ImportContent( string sourceDirectory )
		{
			ImportContent( sourceDirectory, "*.*" );
		}

		/// <summary>
		/// Copies content into the workspace ContentDirectory.
		/// Content are files that will be incorporated into the final help file
		/// </summary>
		/// <param name="sourceDirectory">The path to the xontent files</param>
		/// <param name="filter">File filter to use when selecting files to import</param>
		public void ImportContent( string sourceDirectory, string filter )
		{
			if ( !Directory.Exists( sourceDirectory ) )
				throw new ArgumentException( string.Format( "The source location {0} does not exist", sourceDirectory ) );

			DirectoryInfo dir = new DirectoryInfo( sourceDirectory );
			foreach( FileInfo file in dir.GetFiles( filter ) )
				Workspace.ImportFile( file, ContentDirectory );
		}

		/// <summary>
		/// Raises the <see cref="Workspace.ContentFileAdded"/> event
		/// </summary>
		/// <param name="fileName">The name of the file added</param>
		protected virtual void OnContentFileAdded( string fileName )
		{
			if ( ContentFileAdded != null )
				ContentFileAdded( fileName );
		}

		/// <summary>
		/// Deletes the content of the <see cref="WorkingDirectory"/>
		/// </summary>
		public void CleanIntermediates()
		{
			// delete the resource directory
			if ( Directory.Exists( this.ResourceDirectory ) )
				Directory.Delete( this.ResourceDirectory, true );

			// delete the build temp directory
			if ( Directory.Exists( this.WorkingDirectory ) )
				Directory.Delete( this.WorkingDirectory, true );
		}

		/// <summary>
		/// Delets all output and intermediate files from the project workspace
		/// This will delete all the cleanable files in the root and remove the working directory
		/// </summary>
		public void Clean()
		{
			string[] extenstions = cleanableFileTypes.Split( new char[] { ';' } );

			// first look for any probable project outputs from previous builds in the
			// workspace root and delete them
			foreach ( string ext in extenstions )
			{
				try
				{
					string[] files = Directory.GetFiles( this.RootDirectory, ext );
					foreach ( string f in files)
					{
						FileAttributes attr = File.GetAttributes(f);
						if((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
							File.SetAttributes(f,FileAttributes.Normal);

						File.Delete( f );
					}
				}
				catch (System.IO.IOException) {}
			}

			// then get rid of the working directory
			CleanIntermediates();
		}
	}
}
