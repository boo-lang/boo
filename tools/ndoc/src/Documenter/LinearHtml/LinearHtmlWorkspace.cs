using System;

using NDoc.Core;

namespace NDoc.Documenter.LinearHtml
{
	/// <summary>
	/// Summary description for LatexWorkspace.
	/// </summary>
	public class LinearHtmlWorkspace : Workspace
	{
		/// <summary>
		/// Manages the location of the documentation build process
		/// </summary>
		/// <param name="rootDir">The location to create the workspace</param>
		public LinearHtmlWorkspace( string rootDir ) : base( rootDir, "linearhtml", ".", "" )
		{

		}
	}
}
