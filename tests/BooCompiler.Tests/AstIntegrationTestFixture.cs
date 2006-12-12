namespace BooCompiler.Tests	
{
	using NUnit.Framework;

	[TestFixture]
	public class AstIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void ast_literals_1()
		{
			RunCompilerTestCase(@"e:\projects\boo\tests\testcases\integration\ast\ast-literals-1.boo");
		}
		
		[Test]
		public void ast_literals_2()
		{
			RunCompilerTestCase(@"e:\projects\boo\tests\testcases\integration\ast\ast-literals-2.boo");
		}
		
		[Test]
		public void compile_1()
		{
			RunCompilerTestCase(@"e:\projects\boo\tests\testcases\integration\ast\compile-1.boo");
		}
		
	}
}
