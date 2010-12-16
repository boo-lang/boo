namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ClrExtensionsIntegrationTestFixture : AbstractCompilerTestCase
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
		public void infer_closure_singature()
		{
			RunCompilerTestCase(@"infer-closure-singature.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/clr-extensions";
		}
	}
}
