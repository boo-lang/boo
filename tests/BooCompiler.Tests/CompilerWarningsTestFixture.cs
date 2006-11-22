
namespace BooCompiler.Tests
{
	using NUnit.Framework;
	using Boo.Lang.Compiler;	

	[TestFixture]
	public class CompilerWarningsTestFixture : AbstractCompilerTestCase
	{	
		protected override CompilerPipeline SetUpCompilerPipeline()
		{
			CompilerPipeline pipeline = new Boo.Lang.Compiler.Pipelines.Compile();
			pipeline.Add(new Boo.Lang.Compiler.Steps.PrintWarnings());
			return pipeline;
		}

		[Test]
		public void BCW0001_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-1.boo");
		}
		
		[Test]
		public void BCW0001_10()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-10.boo");
		}
		
		[Test]
		public void BCW0001_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-2.boo");
		}
		
		[Test]
		public void BCW0001_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-3.boo");
		}
		
		[Test]
		public void BCW0001_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-4.boo");
		}
		
		[Test]
		public void BCW0001_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-5.boo");
		}
		
		[Test]
		public void BCW0001_6()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-6.boo");
		}
		
		[Test]
		public void BCW0001_7()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-7.boo");
		}
		
		[Test]
		public void BCW0001_8()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-8.boo");
		}
		
		[Test]
		public void BCW0001_9()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0001-9.boo");
		}
		
		[Test]
		public void BCW0002_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0002-1.boo");
		}
		
		[Test]
		public void BCW0003_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0003-1.boo");
		}
		
		[Test]
		public void BCW0003_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0003-2.boo");
		}
		
		[Test]
		public void BCW0004_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0004-1.boo");
		}
		
		[Test]
		public void BCW0005_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0005-1.boo");
		}
		
		[Test]
		public void BCW0006_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0006-1.boo");
		}
		
		[Test]
		public void BCW0006_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0006-2.boo");
		}
		
		[Test]
		public void BCW0007_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0007-1.boo");
		}
		
		[Test]
		public void BCW0008_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\warnings\BCW0008-1.boo");
		}
		
	}
}
