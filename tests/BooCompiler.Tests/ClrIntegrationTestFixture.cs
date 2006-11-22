namespace BooCompiler.Tests	
{
	using NUnit.Framework;

	[TestFixture]
	public class ClrIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void RealProxy_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\clr\RealProxy-1.boo");
		}
		
	}
}
