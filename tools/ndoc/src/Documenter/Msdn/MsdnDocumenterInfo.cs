using System;

using NDoc.Core;

namespace NDoc.Documenter.Msdn
{
	/// <summary>
	/// Information about the Xml Documenter
	/// </summary>
	public class MsdnDocumenterInfo : BaseDocumenterInfo
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		public MsdnDocumenterInfo() : base( "MSDN" )
		{
		}

		/// <summary>
		/// See <see cref="IDocumenterInfo.CreateConfig()"/>
		/// </summary>
		/// <returns>A config instance</returns>
		public override IDocumenterConfig CreateConfig()
		{
			return new MsdnDocumenterConfig( this );
		}
	}
}