
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
			CompilerPipeline pipeline = new Boo.Lang.Compiler.Pipelines.ExpandMacros();
			pipeline.Add(new PrintBoo());
			return pipeline;
		}


		[Test]
		public void allparametersrequired()
		{
			RunCompilerTestCase(@"allparametersrequired.boo");
		}
		
		[Test]
		public void default_1()
		{
			RunCompilerTestCase(@"default-1.boo");
		}
		
		[Test]
		public void default_2()
		{
			RunCompilerTestCase(@"default-2.boo");
		}
		
		[Test]
		public void getter_1()
		{
			RunCompilerTestCase(@"getter-1.boo");
		}
		
		[Test]
		public void property_1()
		{
			RunCompilerTestCase(@"property-1.boo");
		}
		
		[Test]
		public void property_2()
		{
			RunCompilerTestCase(@"property-2.boo");
		}
		
		[Test]
		public void property_3()
		{
			RunCompilerTestCase(@"property-3.boo");
		}
		
		[Test]
		public void property_4()
		{
			RunCompilerTestCase(@"property-4.boo");
		}
		
		[Test]
		public void property_5()
		{
			RunCompilerTestCase(@"property-5.boo");
		}
		
		[Test]
		public void property_6()
		{
			RunCompilerTestCase(@"property-6.boo");
		}
		
		[Test]
		public void required_1()
		{
			RunCompilerTestCase(@"required-1.boo");
		}
		
		[Test]
		public void required_2()
		{
			RunCompilerTestCase(@"required-2.boo");
		}
		
		[Test]
		public void required_3()
		{
			RunCompilerTestCase(@"required-3.boo");
		}
		
		[Test]
		public void required_4()
		{
			RunCompilerTestCase(@"required-4.boo");
		}
		
		[Test]
		public void required_5()
		{
			RunCompilerTestCase(@"required-5.boo");
		}
		
		[Test]
		public void viewstate()
		{
			RunCompilerTestCase(@"viewstate.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "attributes";
		}
	}
}
