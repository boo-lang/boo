namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class LinqIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void array_linq()
		{
			RunCompilerTestCase(@"array-linq.boo");
		}
		
		[Test]
		public void linq_aggregate()
		{
			RunCompilerTestCase(@"linq-aggregate.boo");
		}
		
		[Test]
		public void linq_extensions_1()
		{
			RunCompilerTestCase(@"linq-extensions-1.boo");
		}
		
		[Test]
		public void linq_extensions_2()
		{
			RunCompilerTestCase(@"linq-extensions-2.boo");
		}
		
		[Test]
		public void overload_resolution_extension()
		{
			RunCompilerTestCase(@"overload-resolution-extension.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/linq";
		}
	}
}
