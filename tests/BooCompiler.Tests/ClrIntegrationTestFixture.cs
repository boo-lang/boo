namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ClrIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void RealProxy_1()
		{
			RunCompilerTestCase(@"RealProxy-1.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/clr";
		}
	}
}
