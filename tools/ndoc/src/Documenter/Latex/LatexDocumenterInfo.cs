using System;

using NDoc.Core;

namespace NDoc.Documenter.Latex
{
	/// <summary>
	/// Information about the Xml Documenter
	/// </summary>
	public class LatexDocumenterInfo : BaseDocumenterInfo
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		public LatexDocumenterInfo() : base( "LaTeX", DocumenterDevelopmentStatus.Stable )
		{
		}

		/// <summary>
		/// See <see cref="IDocumenterInfo.CreateConfig()"/>
		/// </summary>
		/// <returns>A config instance</returns>
		public override IDocumenterConfig CreateConfig()
		{
			return new LatexDocumenterConfig( this );
		}
	}
}