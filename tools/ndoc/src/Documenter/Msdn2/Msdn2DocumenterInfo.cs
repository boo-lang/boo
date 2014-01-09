using System;

using NDoc.Core;

namespace NDoc.Documenter.Msdn2
{
	/// <summary>
	/// Information about the Xml Documenter
	/// </summary>
	public class Msdn2DocumenterInfo : BaseDocumenterInfo
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		public Msdn2DocumenterInfo() : base( "MSDN 2003", DocumenterDevelopmentStatus.Beta )
		{
		}

		/// <summary>
		/// See <see cref="IDocumenterInfo.CreateConfig()"/>
		/// </summary>
		/// <returns>A config instance</returns>
		public override IDocumenterConfig CreateConfig()
		{
			return new Msdn2DocumenterConfig( this );
		}
	}
}