namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ClrextensionsIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void clrextensions_1()
		{
			if (System.Environment.Version < new System.Version(3, 5)) return;
			RunCompilerTestCase(@"clrextensions-1.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/clrextensions";
		}
	}
}
