
namespace BooCompiler.Tests
{
	using NUnit.Framework;
	
	[TestFixture]		
	public class Net2ErrorsTestFixture : AbstractCompilerErrorsTestFixture
	{
		override protected void RunCompilerTestCase(string name)
		{
			if (System.Environment.Version.Major < 2) Assert.Ignore("Test requires .net 2.");
			base.RunCompilerTestCase(name);
		}

		[Test]
		public void BCE0138_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\net2\errors\BCE0138-1.boo");
		}
		
		[Test]
		public void BCE0139_1()
		{
			RunCompilerTestCase(@"c:\dev\boo\tests\testcases\net2\errors\BCE0139-1.boo");
		}
		
	}
}
