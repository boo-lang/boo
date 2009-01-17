namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ClrextensionsIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void clrextensions_1()
		{
			RunCompilerTestCase(@"clrextensions-1.boo");
		}
		
		[Test]
		public void clrextensions_2()
		{
			RunCompilerTestCase(@"clrextensions-2.boo");
		}
		
		[Test]
		public void linq_extensions_1()
		{
			RunCompilerTestCase(@"linq-extensions-1.boo");
		}
		
		[Ignore(" Inferring closure signatures when used as an argument in an overloaded method invocation is not yet supported.")][Test]
		public void linq_extensions_2()
		{
			RunCompilerTestCase(@"linq-extensions-2.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/clrextensions";
		}
	}
}
