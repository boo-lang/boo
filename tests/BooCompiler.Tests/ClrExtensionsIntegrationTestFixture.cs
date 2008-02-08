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
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/clrextensions";
		}
	}
}
