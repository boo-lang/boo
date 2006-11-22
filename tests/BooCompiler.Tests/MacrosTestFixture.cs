
namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class MacrosTestFixture : AbstractCompilerTestCase
	{

		[Test]
		public void assert_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\assert-1.boo");
		}
		
		[Test]
		public void debug_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\debug-1.boo");
		}
		
		[Test]
		public void print_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\print-1.boo");
		}
		
		[Test]
		public void using_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\using-1.boo");
		}
		
		[Test]
		public void using_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\using-2.boo");
		}
		
		[Test]
		public void using_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\using-3.boo");
		}
		
		[Test]
		public void using_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\using-4.boo");
		}
		
		[Test]
		public void using_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\using-5.boo");
		}
		
		[Test]
		public void yieldAll_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\macros\yieldAll-1.boo");
		}
		
	}
}
