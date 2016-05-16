using System;

namespace NDoc.Test.Extensibility
{
	/// <summary>
	/// This namespace is used to test the extensibility feature
	/// see <a href="extend-ndoc.xslt"/>
	/// </summary>
	public class NamespaceDoc {}

	/// <summary>
	/// When processed by the VS.NET or MSDN documenters, using the stylesheet "extend-ndoc.xslt"
	/// as the ExtensibilityStylesheet property will result in end-user defined tags
	/// being displayed in the final help output topics
	/// </summary>
	/// <remarks>This is a test of an inline <null/> tag</remarks>
	/// <custom>This is a custom tag</custom>
	/// <mySeeAlso>This should appear in the "See Also" section</mySeeAlso>
	/// <history><date>2004-10-01</date><user>ksd</user><scr>17482</scr>
	/// Initial comment
	/// </history>
	/// <history><date>2004-10-02</date><user>ksd</user><scr>18212</scr>
	/// Some adiitional comments
	/// </history>
	public class ABunchOfCustomTags
	{

	}
}
