
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
		public void BCW0001_10()
		{
			RunCompilerTestCase(@"BCW0001-10.boo");
		}
		
		[Test]
		public void BCW0001_4()
		{
			RunCompilerTestCase(@"BCW0001-4.boo");
		}
		
		[Test]
		public void BCW0002_1()
		{
			RunCompilerTestCase(@"BCW0002-1.boo");
		}
		
		[Test]
		public void BCW0003_1()
		{
			RunCompilerTestCase(@"BCW0003-1.boo");
		}
		
		[Test]
		public void BCW0003_2()
		{
			RunCompilerTestCase(@"BCW0003-2.boo");
		}
		
		[Test]
		public void BCW0004_1()
		{
			RunCompilerTestCase(@"BCW0004-1.boo");
		}
		
		[Test]
		public void BCW0004_2()
		{
			RunCompilerTestCase(@"BCW0004-2.boo");
		}
		
		[Test]
		public void BCW0005_1()
		{
			RunCompilerTestCase(@"BCW0005-1.boo");
		}
		
		[Test]
		public void BCW0006_1()
		{
			RunCompilerTestCase(@"BCW0006-1.boo");
		}
		
		[Test]
		public void BCW0006_2()
		{
			RunCompilerTestCase(@"BCW0006-2.boo");
		}
		
		[Test]
		public void BCW0007_1()
		{
			RunCompilerTestCase(@"BCW0007-1.boo");
		}
		
		[Test]
		public void BCW0008_1()
		{
			RunCompilerTestCase(@"BCW0008-1.boo");
		}
		
		[Test]
		public void BCW0011_1()
		{
			RunCompilerTestCase(@"BCW0011-1.boo");
		}
		
		[Test]
		public void BCW0011_10()
		{
			RunCompilerTestCase(@"BCW0011-10.boo");
		}
		
		[Test]
		public void BCW0011_11()
		{
			RunCompilerTestCase(@"BCW0011-11.boo");
		}
		
		[Test]
		public void BCW0011_12()
		{
			RunCompilerTestCase(@"BCW0011-12.boo");
		}
		
		[Test]
		public void BCW0011_13()
		{
			RunCompilerTestCase(@"BCW0011-13.boo");
		}
		
		[Test]
		public void BCW0011_14()
		{
			RunCompilerTestCase(@"BCW0011-14.boo");
		}
		
		[Test]
		public void BCW0011_15()
		{
			RunCompilerTestCase(@"BCW0011-15.boo");
		}
		
		[Test]
		public void BCW0011_16()
		{
			RunCompilerTestCase(@"BCW0011-16.boo");
		}
		
		[Test]
		public void BCW0011_17()
		{
			RunCompilerTestCase(@"BCW0011-17.boo");
		}
		
		[Test]
		public void BCW0011_18()
		{
			RunCompilerTestCase(@"BCW0011-18.boo");
		}
		
		[Test]
		public void BCW0011_19()
		{
			RunCompilerTestCase(@"BCW0011-19.boo");
		}
		
		[Test]
		public void BCW0011_2()
		{
			RunCompilerTestCase(@"BCW0011-2.boo");
		}
		
		[Test]
		public void BCW0011_3()
		{
			RunCompilerTestCase(@"BCW0011-3.boo");
		}
		
		[Test]
		public void BCW0011_4()
		{
			RunCompilerTestCase(@"BCW0011-4.boo");
		}
		
		[Test]
		public void BCW0011_5()
		{
			RunCompilerTestCase(@"BCW0011-5.boo");
		}
		
		[Test]
		public void BCW0011_6()
		{
			RunCompilerTestCase(@"BCW0011-6.boo");
		}
		
		[Test]
		public void BCW0011_7()
		{
			RunCompilerTestCase(@"BCW0011-7.boo");
		}
		
		[Test]
		public void BCW0011_8()
		{
			RunCompilerTestCase(@"BCW0011-8.boo");
		}
		
		[Test]
		public void BCW0011_9()
		{
			RunCompilerTestCase(@"BCW0011-9.boo");
		}
		
		[Test]
		public void BCW0012_1()
		{
			RunCompilerTestCase(@"BCW0012-1.boo");
		}
		
		[Test]
		public void BCW0013_1()
		{
			RunCompilerTestCase(@"BCW0013-1.boo");
		}
		
		[Test]
		public void BCW0013_2()
		{
			RunCompilerTestCase(@"BCW0013-2.boo");
		}
		
		[Test]
		public void BCW0013_3()
		{
			RunCompilerTestCase(@"BCW0013-3.boo");
		}
		
		[Test]
		public void BCW0014_1()
		{
			RunCompilerTestCase(@"BCW0014-1.boo");
		}
		
		[Test]
		public void BCW0014_2()
		{
			RunCompilerTestCase(@"BCW0014-2.boo");
		}
		
		[Test]
		public void BCW0015_1()
		{
			RunCompilerTestCase(@"BCW0015-1.boo");
		}
		
		[Test]
		public void BCW0015_2()
		{
			RunCompilerTestCase(@"BCW0015-2.boo");
		}
		
		[Test]
		public void BCW0015_3()
		{
			RunCompilerTestCase(@"BCW0015-3.boo");
		}
		
		[Test]
		public void BCW0015_4()
		{
			RunCompilerTestCase(@"BCW0015-4.boo");
		}
		
		[Test]
		public void BCW0015_5()
		{
			RunCompilerTestCase(@"BCW0015-5.boo");
		}
		
		[Test]
		public void BCW0016_1()
		{
			RunCompilerTestCase(@"BCW0016-1.boo");
		}
		
		[Test]
		public void BCW0017_1()
		{
			RunCompilerTestCase(@"BCW0017-1.boo");
		}
		
		[Test]
		public void BCW0018_1()
		{
			RunCompilerTestCase(@"BCW0018-1.boo");
		}
		
		[Test]
		public void BCW0019_1()
		{
			RunCompilerTestCase(@"BCW0019-1.boo");
		}
		
		[Test]
		public void BCW0020_1()
		{
			RunCompilerTestCase(@"BCW0020-1.boo");
		}
		
		[Test]
		public void BCW0021_1()
		{
			RunCompilerTestCase(@"BCW0021-1.boo");
		}
		
		[Test]
		public void BCW0022_1()
		{
			RunCompilerTestCase(@"BCW0022-1.boo");
		}
		
		[Test]
		public void BCW0022_2()
		{
			RunCompilerTestCase(@"BCW0022-2.boo");
		}
		
		[Test]
		public void BCW0023_1()
		{
			RunCompilerTestCase(@"BCW0023-1.boo");
		}
		
		[Test]
		public void BCW0024_1()
		{
			RunCompilerTestCase(@"BCW0024-1.boo");
		}
		
		[Test]
		public void BCW0025_1()
		{
			RunCompilerTestCase(@"BCW0025-1.boo");
		}
		
		[Test]
		public void BCW0026_1()
		{
			RunCompilerTestCase(@"BCW0026-1.boo");
		}
		
		[Test]
		public void BCW0027_1()
		{
			RunCompilerTestCase(@"BCW0027-1.boo");
		}
		
		[Test]
		public void BCW0028_1()
		{
			RunCompilerTestCase(@"BCW0028-1.boo");
		}
		
		[Test]
		public void BCW0028_2()
		{
			RunCompilerTestCase(@"BCW0028-2.boo");
		}
		
		[Test]
		public void BCW0029_1()
		{
			RunCompilerTestCase(@"BCW0029-1.boo");
		}
		
		[Test]
		public void no_unreacheable_code_warning_for_compiler_generated_code()
		{
			RunCompilerTestCase(@"no-unreacheable-code-warning-for-compiler-generated-code.boo");
		}
		
		[Test]
		public void nowarn_1()
		{
			RunCompilerTestCase(@"nowarn-1.boo");
		}
		
		[Test]
		public void nowarn_2()
		{
			RunCompilerTestCase(@"nowarn-2.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "warnings";
		}
	}
}
