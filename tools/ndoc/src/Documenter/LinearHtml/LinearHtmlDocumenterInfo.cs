using System;

using NDoc.Core;

namespace NDoc.Documenter.LinearHtml
{
	/// <summary>
	/// Information about the Xml Documenter
	/// </summary>
	public class LinearHtmlDocumenterInfo : BaseDocumenterInfo
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		public LinearHtmlDocumenterInfo() : base( "Linear Html", DocumenterDevelopmentStatus.Alpha )
		{
		}

		/// <summary>
		/// See <see cref="IDocumenterInfo.CreateConfig()"/>
		/// </summary>
		/// <returns>A config instance</returns>
		public override IDocumenterConfig CreateConfig()
		{
			return new LinearHtmlDocumenterConfig( this );
		}
	}
}