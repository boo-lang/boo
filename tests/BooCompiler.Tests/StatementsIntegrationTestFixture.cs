namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class StatementsIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void break_1()
		{
			RunCompilerTestCase(@"break-1.boo");
		}
		
		[Test]
		public void break_2()
		{
			RunCompilerTestCase(@"break-2.boo");
		}
		
		[Test]
		public void continue_1()
		{
			RunCompilerTestCase(@"continue-1.boo");
		}
		
		[Test]
		public void continue_2()
		{
			RunCompilerTestCase(@"continue-2.boo");
		}
		
		[Test]
		public void declaration_1()
		{
			RunCompilerTestCase(@"declaration-1.boo");
		}
		
		[Test]
		public void declaration_2()
		{
			RunCompilerTestCase(@"declaration-2.boo");
		}
		
		[Test]
		public void declaration_3()
		{
			RunCompilerTestCase(@"declaration-3.boo");
		}
		
		[Test]
		public void except_1()
		{
			RunCompilerTestCase(@"except-1.boo");
		}
		
		[Test]
		public void except_10()
		{
			RunCompilerTestCase(@"except-10.boo");
		}
		
		[Test]
		public void except_11()
		{
			RunCompilerTestCase(@"except-11.boo");
		}
		
		[Test]
		public void except_12()
		{
			RunCompilerTestCase(@"except-12.boo");
		}
		
		[Test]
		public void except_13()
		{
			RunCompilerTestCase(@"except-13.boo");
		}
		
		[Test]
		public void except_14()
		{
			RunCompilerTestCase(@"except-14.boo");
		}
		
		[Test]
		public void except_2()
		{
			RunCompilerTestCase(@"except-2.boo");
		}
		
		[Test]
		public void except_3()
		{
			RunCompilerTestCase(@"except-3.boo");
		}
		
		[Test]
		public void except_4()
		{
			RunCompilerTestCase(@"except-4.boo");
		}
		
		[Test]
		public void except_5()
		{
			RunCompilerTestCase(@"except-5.boo");
		}
		
		[Test]
		public void except_6()
		{
			RunCompilerTestCase(@"except-6.boo");
		}
		
		[Test]
		public void except_7()
		{
			RunCompilerTestCase(@"except-7.boo");
		}
		
		[Test]
		public void except_8()
		{
			RunCompilerTestCase(@"except-8.boo");
		}
		
		[Test]
		public void except_9()
		{
			RunCompilerTestCase(@"except-9.boo");
		}
		
		[Test]
		public void failure_1()
		{
			RunCompilerTestCase(@"failure-1.boo");
		}
		
		[Test]
		public void failure_2()
		{
			RunCompilerTestCase(@"failure-2.boo");
		}
		
		[Test]
		public void failure_3()
		{
			RunCompilerTestCase(@"failure-3.boo");
		}
		
		[Test]
		public void failure_4()
		{
			RunCompilerTestCase(@"failure-4.boo");
		}
		
		[Test]
		public void failure_5()
		{
			RunCompilerTestCase(@"failure-5.boo");
		}
		
		[Test]
		public void failure_6()
		{
			RunCompilerTestCase(@"failure-6.boo");
		}
		
		[Test]
		public void filter_1()
		{
			RunCompilerTestCase(@"filter-1.boo");
		}
		
		[Test]
		public void filter_2()
		{
			RunCompilerTestCase(@"filter-2.boo");
		}
		
		[Test]
		public void filter_3()
		{
			RunCompilerTestCase(@"filter-3.boo");
		}
		
		[Test]
		public void for_1()
		{
			RunCompilerTestCase(@"for-1.boo");
		}
		
		[Test]
		public void for_10()
		{
			RunCompilerTestCase(@"for-10.boo");
		}
		
		[Test]
		public void for_2()
		{
			RunCompilerTestCase(@"for-2.boo");
		}
		
		[Test]
		public void for_3()
		{
			RunCompilerTestCase(@"for-3.boo");
		}
		
		[Test]
		public void for_4()
		{
			RunCompilerTestCase(@"for-4.boo");
		}
		
		[Test]
		public void for_5()
		{
			RunCompilerTestCase(@"for-5.boo");
		}
		
		[Test]
		public void for_6()
		{
			RunCompilerTestCase(@"for-6.boo");
		}
		
		[Test]
		public void for_6B()
		{
			RunCompilerTestCase(@"for-6B.boo");
		}
		
		[Test]
		public void for_7()
		{
			RunCompilerTestCase(@"for-7.boo");
		}
		
		[Category("FailsOnMono")][Test]
		public void for_8()
		{
			RunCompilerTestCase(@"for-8.boo");
		}
		
		[Test]
		public void for_9()
		{
			RunCompilerTestCase(@"for-9.boo");
		}
		
		[Test]
		public void for_array_1()
		{
			RunCompilerTestCase(@"for-array-1.boo");
		}
		
		[Test]
		public void for_array_2()
		{
			RunCompilerTestCase(@"for-array-2.boo");
		}
		
		[Test]
		public void for_var_reuse()
		{
			RunCompilerTestCase(@"for-var-reuse.boo");
		}
		
		[Test]
		public void for_or_1()
		{
			RunCompilerTestCase(@"for_or-1.boo");
		}
		
		[Test]
		public void for_or_2()
		{
			RunCompilerTestCase(@"for_or-2.boo");
		}
		
		[Test]
		public void for_or_3()
		{
			RunCompilerTestCase(@"for_or-3.boo");
		}
		
		[Test]
		public void for_or_4()
		{
			RunCompilerTestCase(@"for_or-4.boo");
		}
		
		[Test]
		public void for_or_then_1()
		{
			RunCompilerTestCase(@"for_or_then-1.boo");
		}
		
		[Test]
		public void for_or_then_3()
		{
			RunCompilerTestCase(@"for_or_then-3.boo");
		}
		
		[Test]
		public void for_or_then_4()
		{
			RunCompilerTestCase(@"for_or_then-4.boo");
		}
		
		[Test]
		public void for_or_then_5()
		{
			RunCompilerTestCase(@"for_or_then-5.boo");
		}
		
		[Test]
		public void for_then_1()
		{
			RunCompilerTestCase(@"for_then-1.boo");
		}
		
		[Test]
		public void for_then_2()
		{
			RunCompilerTestCase(@"for_then-2.boo");
		}
		
		[Test]
		public void for_then_3()
		{
			RunCompilerTestCase(@"for_then-3.boo");
		}
		
		[Test]
		public void for_then_4()
		{
			RunCompilerTestCase(@"for_then-4.boo");
		}
		
		[Test]
		public void goto_1()
		{
			RunCompilerTestCase(@"goto-1.boo");
		}
		
		[Test]
		public void goto_2()
		{
			RunCompilerTestCase(@"goto-2.boo");
		}
		
		[Test]
		public void goto_3()
		{
			RunCompilerTestCase(@"goto-3.boo");
		}
		
		[Test]
		public void goto_4()
		{
			RunCompilerTestCase(@"goto-4.boo");
		}
		
		[Test]
		public void goto_5()
		{
			RunCompilerTestCase(@"goto-5.boo");
		}
		
		[Test]
		public void goto_6()
		{
			RunCompilerTestCase(@"goto-6.boo");
		}
		
		[Test]
		public void raise_1()
		{
			RunCompilerTestCase(@"raise-1.boo");
		}
		
		[Test]
		public void reraise_1()
		{
			RunCompilerTestCase(@"reraise-1.boo");
		}
		
		[Test]
		public void reraise_2()
		{
			RunCompilerTestCase(@"reraise-2.boo");
		}
		
		[Test]
		public void try_1()
		{
			RunCompilerTestCase(@"try-1.boo");
		}
		
		[Test]
		public void try_2()
		{
			RunCompilerTestCase(@"try-2.boo");
		}
		
		[Test]
		public void try_3()
		{
			RunCompilerTestCase(@"try-3.boo");
		}
		
		[Test]
		public void unpack_1()
		{
			RunCompilerTestCase(@"unpack-1.boo");
		}
		
		[Test]
		public void unpack_10()
		{
			RunCompilerTestCase(@"unpack-10.boo");
		}
		
		[Test]
		public void unpack_11()
		{
			RunCompilerTestCase(@"unpack-11.boo");
		}
		
		[Test]
		public void unpack_12()
		{
			RunCompilerTestCase(@"unpack-12.boo");
		}
		
		[Test]
		public void unpack_13()
		{
			RunCompilerTestCase(@"unpack-13.boo");
		}
		
		[Test]
		public void unpack_2()
		{
			RunCompilerTestCase(@"unpack-2.boo");
		}
		
		[Test]
		public void unpack_3()
		{
			RunCompilerTestCase(@"unpack-3.boo");
		}
		
		[Test]
		public void unpack_4()
		{
			RunCompilerTestCase(@"unpack-4.boo");
		}
		
		[Test]
		public void unpack_5()
		{
			RunCompilerTestCase(@"unpack-5.boo");
		}
		
		[Test]
		public void unpack_6()
		{
			RunCompilerTestCase(@"unpack-6.boo");
		}
		
		[Test]
		public void unpack_7()
		{
			RunCompilerTestCase(@"unpack-7.boo");
		}
		
		[Test]
		public void unpack_8()
		{
			RunCompilerTestCase(@"unpack-8.boo");
		}
		
		[Test]
		public void unpack_9()
		{
			RunCompilerTestCase(@"unpack-9.boo");
		}
		
		[Test]
		public void while_1()
		{
			RunCompilerTestCase(@"while-1.boo");
		}
		
		[Test]
		public void while_2()
		{
			RunCompilerTestCase(@"while-2.boo");
		}
		
		[Test]
		public void while_3()
		{
			RunCompilerTestCase(@"while-3.boo");
		}
		
		[Test]
		public void while_4()
		{
			RunCompilerTestCase(@"while-4.boo");
		}
		
		[Test]
		public void while_5()
		{
			RunCompilerTestCase(@"while-5.boo");
		}
		
		[Test]
		public void while_6()
		{
			RunCompilerTestCase(@"while-6.boo");
		}
		
		[Test]
		public void while_7()
		{
			RunCompilerTestCase(@"while-7.boo");
		}
		
		[Test]
		public void while_8()
		{
			RunCompilerTestCase(@"while-8.boo");
		}
		
		[Test]
		public void while_or_1()
		{
			RunCompilerTestCase(@"while_or-1.boo");
		}
		
		[Test]
		public void while_or_2()
		{
			RunCompilerTestCase(@"while_or-2.boo");
		}
		
		[Test]
		public void while_or_3()
		{
			RunCompilerTestCase(@"while_or-3.boo");
		}
		
		[Test]
		public void while_or_4()
		{
			RunCompilerTestCase(@"while_or-4.boo");
		}
		
		[Test]
		public void while_or_then_1()
		{
			RunCompilerTestCase(@"while_or_then-1.boo");
		}
		
		[Test]
		public void while_or_then_4()
		{
			RunCompilerTestCase(@"while_or_then-4.boo");
		}
		
		[Test]
		public void while_or_then_5()
		{
			RunCompilerTestCase(@"while_or_then-5.boo");
		}
		
		[Test]
		public void while_then_1()
		{
			RunCompilerTestCase(@"while_then-1.boo");
		}
		
		[Test]
		public void while_then_2()
		{
			RunCompilerTestCase(@"while_then-2.boo");
		}
		
		[Test]
		public void while_then_3()
		{
			RunCompilerTestCase(@"while_then-3.boo");
		}
		
		[Test]
		public void while_then_4()
		{
			RunCompilerTestCase(@"while_then-4.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/statements";
		}
	}
}
