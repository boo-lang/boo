using System;

using NDoc.Core;

namespace NDoc.Documenter.JavaDoc
{
	/// <summary>
	/// Summary description for LatexWorkspace.
	/// </summary>
	public class JavaDocWorkspace : Workspace
	{
		/// <summary>
		/// Manages the location of the documentation build process
		/// </summary>
		/// <param name="rootDir">The location to create the workspace</param>
		public JavaDocWorkspace( string rootDir ) : base( rootDir, "javadoc", ".", "" )
		{

		}
	}
}
