namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ModulesIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void dotted_module_name()
		{
			RunCompilerTestCase(@"dotted.module.name.boo");
		}
		
		[Test]
		public void import_1()
		{
			RunCompilerTestCase(@"import-1.boo");
		}
		
		[Test]
		public void import_2()
		{
			RunCompilerTestCase(@"import-2.boo");
		}
		
		[Test]
		public void import_3()
		{
			RunCompilerTestCase(@"import-3.boo");
		}
		
		[Test]
		public void import_4()
		{
			RunCompilerTestCase(@"import-4.boo");
		}
		
		[Test]
		public void import_5()
		{
			RunCompilerTestCase(@"import-5.boo");
		}
		
		[Test]
		public void import_6()
		{
			RunCompilerTestCase(@"import-6.boo");
		}
		
		[Test]
		public void import_7()
		{
			RunCompilerTestCase(@"import-7.boo");
		}
		
		[Test]
		public void import_8()
		{
			RunCompilerTestCase(@"import-8.boo");
		}
		
		[Test]
		public void import_9()
		{
			RunCompilerTestCase(@"import-9.boo");
		}
		
		[Test]
		public void modules_1()
		{
			RunCompilerTestCase(@"modules-1.boo");
		}
		
		[Test]
		public void modules_2()
		{
			RunCompilerTestCase(@"modules-2.boo");
		}
		
		[Test]
		public void modules_3()
		{
			RunCompilerTestCase(@"modules-3.boo");
		}
		
		[Test]
		public void modules_4()
		{
			RunCompilerTestCase(@"modules-4.boo");
		}
		
		[Test]
		public void modules_5()
		{
			RunCompilerTestCase(@"modules-5.boo");
		}
		
		[Test]
		public void modules_6()
		{
			RunCompilerTestCase(@"modules-6.boo");
		}
		
		[Test]
		public void peek_a()
		{
			RunCompilerTestCase(@"peek.a.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/modules";
		}
	}
}
