using System;

using NDoc.Core;

namespace NDoc.Documenter.Xml
{
	/// <summary>
	/// Information about the Xml Documenter
	/// </summary>
	public class XmlDocumenterInfo : BaseDocumenterInfo
	{
		/// <summary>
		/// Creates a new instance of the class
		/// </summary>
		public XmlDocumenterInfo() : base( "XML" )
		{
		}

		/// <summary>
		/// See <see cref="IDocumenterInfo.CreateConfig()"/>
		/// </summary>
		/// <returns>A config instance</returns>
		public override IDocumenterConfig CreateConfig()
		{
			return new XmlDocumenterConfig( this );
		}
	}
}