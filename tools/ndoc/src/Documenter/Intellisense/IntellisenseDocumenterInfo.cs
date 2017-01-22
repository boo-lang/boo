using System;

using NDoc.Core;

namespace NDoc.Documenter.Intellisense
{
	/// <summary>
	/// Information about the Documenter
	/// </summary>
	public class IntellisenseDocumenterInfo : BaseDocumenterInfo
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		public IntellisenseDocumenterInfo() : base( "Intellisense" )
		{
		}

		/// <summary>
		/// See <see cref="IDocumenterInfo.CreateConfig"/>
		/// </summary>
		/// <returns>A config instance</returns>
		public override IDocumenterConfig CreateConfig()
		{
			return new IntellisenseDocumenterConfig( this );
		}
	}
}