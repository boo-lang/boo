namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class ClosuresIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void closure_inference_1()
		{
			RunCompilerTestCase(@"closure-inference-1.boo");
		}
		
		[Test]
		public void closure_inference_2()
		{
			RunCompilerTestCase(@"closure-inference-2.boo");
		}
		
		[Test]
		public void closure_inference_3()
		{
			RunCompilerTestCase(@"closure-inference-3.boo");
		}
		
		[Test]
		public void closure_inference_4()
		{
			RunCompilerTestCase(@"closure-inference-4.boo");
		}
		
		[Test]
		public void closure_inference_5()
		{
			RunCompilerTestCase(@"closure-inference-5.boo");
		}
		
		[Test]
		public void closure_inference_6()
		{
			RunCompilerTestCase(@"closure-inference-6.boo");
		}
		
		[Test]
		public void closure_inference_7()
		{
			RunCompilerTestCase(@"closure-inference-7.boo");
		}
		
		[Test]
		public void closures_1()
		{
			RunCompilerTestCase(@"closures-1.boo");
		}
		
		[Test]
		public void closures_10()
		{
			RunCompilerTestCase(@"closures-10.boo");
		}
		
		[Test]
		public void closures_11()
		{
			RunCompilerTestCase(@"closures-11.boo");
		}
		
		[Test]
		public void closures_12()
		{
			RunCompilerTestCase(@"closures-12.boo");
		}
		
		[Test]
		public void closures_13()
		{
			RunCompilerTestCase(@"closures-13.boo");
		}
		
		[Test]
		public void closures_14()
		{
			RunCompilerTestCase(@"closures-14.boo");
		}
		
		[Test]
		public void closures_15()
		{
			RunCompilerTestCase(@"closures-15.boo");
		}
		
		[Test]
		public void closures_16()
		{
			RunCompilerTestCase(@"closures-16.boo");
		}
		
		[Test]
		public void closures_17()
		{
			RunCompilerTestCase(@"closures-17.boo");
		}
		
		[Test]
		public void closures_18()
		{
			RunCompilerTestCase(@"closures-18.boo");
		}
		
		[Test]
		public void closures_19()
		{
			RunCompilerTestCase(@"closures-19.boo");
		}
		
		[Test]
		public void closures_2()
		{
			RunCompilerTestCase(@"closures-2.boo");
		}
		
		[Test]
		public void closures_20()
		{
			RunCompilerTestCase(@"closures-20.boo");
		}
		
		[Test]
		public void closures_21()
		{
			RunCompilerTestCase(@"closures-21.boo");
		}
		
		[Test]
		public void closures_22()
		{
			RunCompilerTestCase(@"closures-22.boo");
		}
		
		[Test]
		public void closures_23()
		{
			RunCompilerTestCase(@"closures-23.boo");
		}
		
		[Test]
		public void closures_24()
		{
			RunCompilerTestCase(@"closures-24.boo");
		}
		
		[Test]
		public void closures_25()
		{
			RunCompilerTestCase(@"closures-25.boo");
		}
		
		[Test]
		public void closures_26()
		{
			RunCompilerTestCase(@"closures-26.boo");
		}
		
		[Test]
		public void closures_27()
		{
			RunCompilerTestCase(@"closures-27.boo");
		}
		
		[Test]
		public void closures_28()
		{
			RunCompilerTestCase(@"closures-28.boo");
		}
		
		[Test]
		public void closures_29()
		{
			RunCompilerTestCase(@"closures-29.boo");
		}
		
		[Test]
		public void closures_3()
		{
			RunCompilerTestCase(@"closures-3.boo");
		}
		
		[Test]
		public void closures_30()
		{
			RunCompilerTestCase(@"closures-30.boo");
		}
		
		[Test]
		public void closures_31()
		{
			RunCompilerTestCase(@"closures-31.boo");
		}
		
		[Test]
		public void closures_4()
		{
			RunCompilerTestCase(@"closures-4.boo");
		}
		
		[Test]
		public void closures_5()
		{
			RunCompilerTestCase(@"closures-5.boo");
		}
		
		[Test]
		public void closures_6()
		{
			RunCompilerTestCase(@"closures-6.boo");
		}
		
		[Test]
		public void closures_7()
		{
			RunCompilerTestCase(@"closures-7.boo");
		}
		
		[Test]
		public void closures_8()
		{
			RunCompilerTestCase(@"closures-8.boo");
		}
		
		[Test]
		public void closures_9()
		{
			RunCompilerTestCase(@"closures-9.boo");
		}
		
		[Test]
		public void explicit_closure_types_for_generic_method()
		{
			RunCompilerTestCase(@"explicit-closure-types-for-generic-method.boo");
		}
		
		[Test]
		public void nested_functions_1()
		{
			RunCompilerTestCase(@"nested-functions-1.boo");
		}
		
		[Test]
		public void nested_functions_2()
		{
			RunCompilerTestCase(@"nested-functions-2.boo");
		}
		
		[Test]
		public void nested_functions_3()
		{
			RunCompilerTestCase(@"nested-functions-3.boo");
		}
		
		[Test]
		public void nested_functions_4()
		{
			RunCompilerTestCase(@"nested-functions-4.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/closures";
		}
	}
}
