using System;

using NDoc.Core;

namespace NDoc.Documenter.NativeHtmlHelp2
{
	/// <summary>
	/// Information about the Xml Documenter
	/// </summary>
	public class NativeHtmlHelp2DocumenterInfo : BaseDocumenterInfo
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		public NativeHtmlHelp2DocumenterInfo() : base( "VS.NET 2003" )
		{
		}

		/// <summary>
		/// See <see cref="IDocumenterInfo.CreateConfig"/>
		/// </summary>
		/// <returns>A config instance</returns>
		public override IDocumenterConfig CreateConfig()
		{
			return new NativeHtmlHelp2Config( this );
		}
	}
}