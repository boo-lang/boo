namespace BooCompiler.Tests.SupportingClasses
{
	public class ThisReturnTypeIsAttribute : System.Attribute
	{
		public ThisReturnTypeIsAttribute(string what)
		{
			What = what;
		}

		public string What { get; set; }
	}
}
