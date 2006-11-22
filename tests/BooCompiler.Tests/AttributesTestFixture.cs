
namespace BooCompiler.Tests
{
	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Steps;
	
	[TestFixture]
	public class AttributesTestFixture : AbstractCompilerTestCase
	{
		override protected CompilerPipeline SetUpCompilerPipeline()
		{
			CompilerPipeline pipeline = new Boo.Lang.Compiler.Pipelines.Parse();
			pipeline.Add(new InitializeTypeSystemServices());
			pipeline.Add(new InitializeNameResolutionService());
			pipeline.Add(new IntroduceGlobalNamespaces());	
			pipeline.Add(new BindNamespaces());
			pipeline.Add(new BindAndApplyAttributes());
			pipeline.Add(new PrintBoo());
			return pipeline;
		}

		[Test]
		public void allparametersrequired()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\allparametersrequired.boo");
		}
		
		[Test]
		public void getter_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\getter-1.boo");
		}
		
		[Test]
		public void property_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\property-1.boo");
		}
		
		[Test]
		public void property_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\property-2.boo");
		}
		
		[Test]
		public void property_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\property-3.boo");
		}
		
		[Test]
		public void property_4()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\property-4.boo");
		}
		
		[Test]
		public void property_5()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\property-5.boo");
		}
		
		[Test]
		public void required_1()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\required-1.boo");
		}
		
		[Test]
		public void required_2()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\required-2.boo");
		}
		
		[Test]
		public void required_3()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\required-3.boo");
		}
		
		[Test]
		public void viewstate()
		{
			RunCompilerTestCase(@"c:\projects\boo\tests\testcases\attributes\viewstate.boo");
		}
		
	}
}
