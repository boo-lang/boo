namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class OperatorsIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void and_1()
		{
			RunCompilerTestCase(@"and-1.boo");
		}
		
		[Test]
		public void and_2()
		{
			RunCompilerTestCase(@"and-2.boo");
		}
		
		[Test]
		public void bitwise_and_1()
		{
			RunCompilerTestCase(@"bitwise-and-1.boo");
		}
		
		[Test]
		public void bitwise_and_2()
		{
			RunCompilerTestCase(@"bitwise-and-2.boo");
		}
		
		[Test]
		public void bitwise_or_1()
		{
			RunCompilerTestCase(@"bitwise-or-1.boo");
		}
		
		[Test]
		public void bitwise_or_2()
		{
			RunCompilerTestCase(@"bitwise-or-2.boo");
		}
		
		[Test]
		public void cast_1()
		{
			RunCompilerTestCase(@"cast-1.boo");
		}
		
		[Test]
		public void cast_2()
		{
			RunCompilerTestCase(@"cast-2.boo");
		}
		
		[Test]
		public void cast_3()
		{
			RunCompilerTestCase(@"cast-3.boo");
		}
		
		[Test]
		public void cast_4()
		{
			RunCompilerTestCase(@"cast-4.boo");
		}
		
		[Test]
		public void cast_5()
		{
			RunCompilerTestCase(@"cast-5.boo");
		}
		
		[Test]
		public void cast_6()
		{
			RunCompilerTestCase(@"cast-6.boo");
		}
		
		[Test]
		public void cast_7()
		{
			RunCompilerTestCase(@"cast-7.boo");
		}
		
		[Test]
		public void cast_8()
		{
			RunCompilerTestCase(@"cast-8.boo");
		}
		
		[Test]
		public void cast_9()
		{
			RunCompilerTestCase(@"cast-9.boo");
		}
		
		[Test]
		public void conditional_1()
		{
			RunCompilerTestCase(@"conditional-1.boo");
		}
		
		[Test]
		public void explode_1()
		{
			RunCompilerTestCase(@"explode-1.boo");
		}
		
		[Test]
		public void explode_2()
		{
			RunCompilerTestCase(@"explode-2.boo");
		}
		
		[Test]
		public void explode_3()
		{
			RunCompilerTestCase(@"explode-3.boo");
		}
		
		[Test]
		public void exponential_1()
		{
			RunCompilerTestCase(@"exponential-1.boo");
		}
		
		[Test]
		public void in_1()
		{
			RunCompilerTestCase(@"in-1.boo");
		}
		
		[Test]
		public void is_1()
		{
			RunCompilerTestCase(@"is-1.boo");
		}
		
		[Test]
		public void is_2()
		{
			RunCompilerTestCase(@"is-2.boo");
		}
		
		[Test]
		public void isa_1()
		{
			RunCompilerTestCase(@"isa-1.boo");
		}
		
		[Test]
		public void isa_2()
		{
			RunCompilerTestCase(@"isa-2.boo");
		}
		
		[Test]
		public void isa_3()
		{
			RunCompilerTestCase(@"isa-3.boo");
		}
		
		[Test]
		public void not_1()
		{
			RunCompilerTestCase(@"not-1.boo");
		}
		
		[Test]
		public void ones_complement_1()
		{
			RunCompilerTestCase(@"ones-complement-1.boo");
		}
		
		[Test]
		public void operators_1()
		{
			RunCompilerTestCase(@"operators-1.boo");
		}
		
		[Test]
		public void or_1()
		{
			RunCompilerTestCase(@"or-1.boo");
		}
		
		[Test]
		public void or_2()
		{
			RunCompilerTestCase(@"or-2.boo");
		}
		
		[Test]
		public void or_3()
		{
			RunCompilerTestCase(@"or-3.boo");
		}
		
		[Test]
		public void or_4()
		{
			RunCompilerTestCase(@"or-4.boo");
		}
		
		[Test]
		public void post_incdec_1()
		{
			RunCompilerTestCase(@"post-incdec-1.boo");
		}
		
		[Test]
		public void post_incdec_2()
		{
			RunCompilerTestCase(@"post-incdec-2.boo");
		}
		
		[Test]
		public void post_incdec_3()
		{
			RunCompilerTestCase(@"post-incdec-3.boo");
		}
		
		[Test]
		public void post_incdec_4()
		{
			RunCompilerTestCase(@"post-incdec-4.boo");
		}
		
		[Test]
		public void post_incdec_5()
		{
			RunCompilerTestCase(@"post-incdec-5.boo");
		}
		
		[Test]
		public void post_incdec_6()
		{
			RunCompilerTestCase(@"post-incdec-6.boo");
		}
		
		[Test]
		public void post_incdec_7()
		{
			RunCompilerTestCase(@"post-incdec-7.boo");
		}
		
		[Test]
		public void safeaccess_1()
		{
			RunCompilerTestCase(@"safeaccess-1.boo");
		}
		
		[Test]
		public void safeaccess_2()
		{
			RunCompilerTestCase(@"safeaccess-2.boo");
		}
		
		[Test]
		public void shift_1()
		{
			RunCompilerTestCase(@"shift-1.boo");
		}
		
		[Test]
		public void slicing_1()
		{
			RunCompilerTestCase(@"slicing-1.boo");
		}
		
		[Test]
		public void slicing_10()
		{
			RunCompilerTestCase(@"slicing-10.boo");
		}
		
		[Test]
		public void slicing_11()
		{
			RunCompilerTestCase(@"slicing-11.boo");
		}
		
		[Test]
		public void slicing_12()
		{
			RunCompilerTestCase(@"slicing-12.boo");
		}
		
		[Test]
		public void slicing_13()
		{
			RunCompilerTestCase(@"slicing-13.boo");
		}
		
		[Test]
		public void slicing_2()
		{
			RunCompilerTestCase(@"slicing-2.boo");
		}
		
		[Test]
		public void slicing_3()
		{
			RunCompilerTestCase(@"slicing-3.boo");
		}
		
		[Test]
		public void slicing_4()
		{
			RunCompilerTestCase(@"slicing-4.boo");
		}
		
		[Test]
		public void slicing_5()
		{
			RunCompilerTestCase(@"slicing-5.boo");
		}
		
		[Test]
		public void slicing_6()
		{
			RunCompilerTestCase(@"slicing-6.boo");
		}
		
		[Test]
		public void slicing_7()
		{
			RunCompilerTestCase(@"slicing-7.boo");
		}
		
		[Test]
		public void slicing_8()
		{
			RunCompilerTestCase(@"slicing-8.boo");
		}
		
		[Test]
		public void slicing_9()
		{
			RunCompilerTestCase(@"slicing-9.boo");
		}
		
		[Test]
		public void slicing_md_1()
		{
			RunCompilerTestCase(@"slicing-md-1.boo");
		}
		
		[Test]
		public void slicing_md_2()
		{
			RunCompilerTestCase(@"slicing-md-2.boo");
		}
		
		[Test]
		public void slicing_md_3()
		{
			RunCompilerTestCase(@"slicing-md-3.boo");
		}
		
		[Test]
		public void slicing_md_4()
		{
			RunCompilerTestCase(@"slicing-md-4.boo");
		}
		
		[Test]
		public void unary_1()
		{
			RunCompilerTestCase(@"unary-1.boo");
		}
		
		[Test]
		public void unary_2()
		{
			RunCompilerTestCase(@"unary-2.boo");
		}
		
		[Test]
		public void unary_3()
		{
			RunCompilerTestCase(@"unary-3.boo");
		}
		
		[Test]
		public void xor_1()
		{
			RunCompilerTestCase(@"xor-1.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/operators";
		}
	}
}
