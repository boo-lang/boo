using System;
using System.Collections;
using System.Xml;

namespace NDoc.Core
{
	/// <summary>
	/// Specifies the development status of a documenter.
	/// </summary>
	public enum DocumenterDevelopmentStatus
	{
		/// <summary>Still in development, not really ready for anyone 
		/// else to use except to provide feedback.</summary>
		Alpha,
		/// <summary>Ready for users to try out, with the understanding that
		/// bugs are likely.</summary>
		Beta,
		/// <summary>Ready for use, or at least as stable as free 
		/// software gets!</summary>
		Stable,
		/// <summary>
		/// No longer actively maintained.
		/// </summary>
		Obsolete
	}

	/// <summary>
	/// Descriptive information about a documenter
	/// </summary>
	public interface IDocumenterInfo
	{
		/// <summary>
		/// Creates an instance of an <see cref="IDocumenterConfig"/>
		/// for the particular type of documenter and associates it with a project
		/// </summary>
		/// <param name="project">A project</param>
		/// <returns>An <see cref="IDocumenterConfig"/> for the documenter type</returns>
		IDocumenterConfig CreateConfig( Project project );

		/// <summary>
		/// Creates an instance of an <see cref="IDocumenterConfig"/>
		/// for the particular type of documenter
		/// </summary>
		/// <returns>An <see cref="IDocumenterConfig"/> for the documenter type</returns>
		IDocumenterConfig CreateConfig();

		/// <summary>
		/// The name of the documenter
		/// </summary>
		string Name{ get; }
		
		/// <summary>
		/// The development status (alpha, beta, stable) of this documenter.
		/// </summary>
		DocumenterDevelopmentStatus DevelopmentStatus { get; }
	}
}