namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class GeneratorsIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void generator_calling_external_super_with_arguments_2()
		{
			RunCompilerTestCase(@"generator-calling-external-super-with-arguments-2.boo");
		}
		
		[Test]
		public void generator_calling_external_super_with_arguments()
		{
			RunCompilerTestCase(@"generator-calling-external-super-with-arguments.boo");
		}
		
		[Test]
		public void generator_calling_super_with_arguments_2()
		{
			RunCompilerTestCase(@"generator-calling-super-with-arguments-2.boo");
		}
		
		[Test]
		public void generator_calling_super_with_arguments()
		{
			RunCompilerTestCase(@"generator-calling-super-with-arguments.boo");
		}
		
		[Test]
		public void generator_calling_super()
		{
			RunCompilerTestCase(@"generator-calling-super.boo");
		}
		
		[Test]
		public void generator_of_static_class_is_transient()
		{
			RunCompilerTestCase(@"generator-of-static-class-is-transient.boo");
		}
		
		[Test]
		public void generator_of_transient_class_is_transient()
		{
			RunCompilerTestCase(@"generator-of-transient-class-is-transient.boo");
		}
		
		[Test]
		public void generators_1()
		{
			RunCompilerTestCase(@"generators-1.boo");
		}
		
		[Test]
		public void generators_10()
		{
			RunCompilerTestCase(@"generators-10.boo");
		}
		
		[Test]
		public void generators_11()
		{
			RunCompilerTestCase(@"generators-11.boo");
		}
		
		[Test]
		public void generators_12()
		{
			RunCompilerTestCase(@"generators-12.boo");
		}
		
		[Test]
		public void generators_13()
		{
			RunCompilerTestCase(@"generators-13.boo");
		}
		
		[Test]
		public void generators_14()
		{
			RunCompilerTestCase(@"generators-14.boo");
		}
		
		[Test]
		public void generators_15()
		{
			RunCompilerTestCase(@"generators-15.boo");
		}
		
		[Test]
		public void generators_16()
		{
			RunCompilerTestCase(@"generators-16.boo");
		}
		
		[Category("FailsOnMono4")][Test]
		public void generators_17()
		{
			RunCompilerTestCase(@"generators-17.boo");
		}
		
		[Test]
		public void generators_18()
		{
			RunCompilerTestCase(@"generators-18.boo");
		}
		
		[Test]
		public void generators_19()
		{
			RunCompilerTestCase(@"generators-19.boo");
		}
		
		[Test]
		public void generators_2()
		{
			RunCompilerTestCase(@"generators-2.boo");
		}
		
		[Test]
		public void generators_20()
		{
			RunCompilerTestCase(@"generators-20.boo");
		}
		
		[Test]
		public void generators_21()
		{
			RunCompilerTestCase(@"generators-21.boo");
		}
		
		[Test]
		public void generators_3()
		{
			RunCompilerTestCase(@"generators-3.boo");
		}
		
		[Test]
		public void generators_4()
		{
			RunCompilerTestCase(@"generators-4.boo");
		}
		
		[Test]
		public void generators_5()
		{
			RunCompilerTestCase(@"generators-5.boo");
		}
		
		[Test]
		public void generators_6()
		{
			RunCompilerTestCase(@"generators-6.boo");
		}
		
		[Test]
		public void generators_7()
		{
			RunCompilerTestCase(@"generators-7.boo");
		}
		
		[Test]
		public void generators_8()
		{
			RunCompilerTestCase(@"generators-8.boo");
		}
		
		[Test]
		public void generators_9()
		{
			RunCompilerTestCase(@"generators-9.boo");
		}
		
		[Ignore("BOO-759 - generic generator methods are not supported")][Test]
		public void generic_generator_1()
		{
			RunCompilerTestCase(@"generic-generator-1.boo");
		}
		
		[Test]
		public void generic_generator_2()
		{
			RunCompilerTestCase(@"generic-generator-2.boo");
		}
		
		[Test]
		public void label_issue_1()
		{
			RunCompilerTestCase(@"label-issue-1.boo");
		}
		
		[Test]
		public void list_generators_1()
		{
			RunCompilerTestCase(@"list-generators-1.boo");
		}
		
		[Test]
		public void list_generators_2()
		{
			RunCompilerTestCase(@"list-generators-2.boo");
		}
		
		[Test]
		public void list_generators_3()
		{
			RunCompilerTestCase(@"list-generators-3.boo");
		}
		
		[Test]
		public void list_generators_4()
		{
			RunCompilerTestCase(@"list-generators-4.boo");
		}
		
		[Test]
		public void list_generators_5()
		{
			RunCompilerTestCase(@"list-generators-5.boo");
		}
		
		[Test]
		public void to_string()
		{
			RunCompilerTestCase(@"to-string.boo");
		}
		
		[Test]
		public void yield_1()
		{
			RunCompilerTestCase(@"yield-1.boo");
		}
		
		[Test]
		public void yield_10()
		{
			RunCompilerTestCase(@"yield-10.boo");
		}
		
		[Test]
		public void yield_11()
		{
			RunCompilerTestCase(@"yield-11.boo");
		}
		
		[Test]
		public void yield_12()
		{
			RunCompilerTestCase(@"yield-12.boo");
		}
		
		[Test]
		public void yield_13()
		{
			RunCompilerTestCase(@"yield-13.boo");
		}
		
		[Test]
		public void yield_14()
		{
			RunCompilerTestCase(@"yield-14.boo");
		}
		
		[Test]
		public void yield_15()
		{
			RunCompilerTestCase(@"yield-15.boo");
		}
		
		[Test]
		public void yield_16()
		{
			RunCompilerTestCase(@"yield-16.boo");
		}
		
		[Test]
		public void yield_17()
		{
			RunCompilerTestCase(@"yield-17.boo");
		}
		
		[Test]
		public void yield_2()
		{
			RunCompilerTestCase(@"yield-2.boo");
		}
		
		[Test]
		public void yield_3()
		{
			RunCompilerTestCase(@"yield-3.boo");
		}
		
		[Test]
		public void yield_4()
		{
			RunCompilerTestCase(@"yield-4.boo");
		}
		
		[Test]
		public void yield_5()
		{
			RunCompilerTestCase(@"yield-5.boo");
		}
		
		[Test]
		public void yield_6()
		{
			RunCompilerTestCase(@"yield-6.boo");
		}
		
		[Test]
		public void yield_7()
		{
			RunCompilerTestCase(@"yield-7.boo");
		}
		
		[Test]
		public void yield_8()
		{
			RunCompilerTestCase(@"yield-8.boo");
		}
		
		[Test]
		public void yield_9()
		{
			RunCompilerTestCase(@"yield-9.boo");
		}
		
		[Test]
		public void yield_null_as_IEnumerator()
		{
			RunCompilerTestCase(@"yield-null-as-IEnumerator.boo");
		}
		
		[Test]
		public void yield_null()
		{
			RunCompilerTestCase(@"yield-null.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/generators";
		}
	}
}
