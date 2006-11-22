namespace BooCompiler.Tests	
{
	using NUnit.Framework;

	[TestFixture]
	public class ModulesIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void import_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-1.boo");
		}
		
		[Test]
		public void import_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-2.boo");
		}
		
		[Test]
		public void import_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-3.boo");
		}
		
		[Test]
		public void import_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-4.boo");
		}
		
		[Test]
		public void import_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-5.boo");
		}
		
		[Test]
		public void import_6()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-6.boo");
		}
		
		[Test]
		public void import_7()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-7.boo");
		}
		
		[Test]
		public void import_8()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\import-8.boo");
		}
		
		[Test]
		public void modules_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\modules-1.boo");
		}
		
		[Test]
		public void modules_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\modules-2.boo");
		}
		
		[Test]
		public void modules_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\modules-3.boo");
		}
		
		[Test]
		public void modules_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\modules-4.boo");
		}
		
		[Test]
		public void modules_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\modules-5.boo");
		}
		
		[Test]
		public void modules_6()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\integration\modules\modules-6.boo");
		}
		
	}
}
