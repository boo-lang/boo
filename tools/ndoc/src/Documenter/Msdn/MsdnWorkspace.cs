using System;

using NDoc.Core;

namespace NDoc.Documenter.Msdn
{
	/// <summary>
	/// Summary description for MsdnWorkspace.
	/// </summary>
	public class MsdnWorkspace : Workspace
	{
		/// <summary>
		/// Contructs a new instance of the MsdnWorkspace class
		/// </summary>
		/// <param name="rootDir">The location to create the workspace</param>
		public MsdnWorkspace( string rootDir ) : base( rootDir, "msdn", ".", "*.chm" )
		{
		}
	}
}
