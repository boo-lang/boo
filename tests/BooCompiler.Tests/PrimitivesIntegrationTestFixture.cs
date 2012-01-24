namespace BooCompiler.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class PrimitivesIntegrationTestFixture : AbstractCompilerTestCase
	{
	
		[Test]
		public void at_operator()
		{
			RunCompilerTestCase(@"at-operator.boo");
		}
		
		[Test]
		public void bool_1()
		{
			RunCompilerTestCase(@"bool-1.boo");
		}
		
		[Test]
		public void char_1()
		{
			RunCompilerTestCase(@"char-1.boo");
		}
		
		[Test]
		public void char_2()
		{
			RunCompilerTestCase(@"char-2.boo");
		}
		
		[Test]
		public void char_3()
		{
			RunCompilerTestCase(@"char-3.boo");
		}
		
		[Test]
		public void char_4()
		{
			RunCompilerTestCase(@"char-4.boo");
		}
		
		[Test]
		public void char_5()
		{
			RunCompilerTestCase(@"char-5.boo");
		}
		
		[Test]
		public void checked_1()
		{
			RunCompilerTestCase(@"checked-1.boo");
		}
		
		[Test]
		public void decimal_1()
		{
			RunCompilerTestCase(@"decimal-1.boo");
		}
		
		[Test]
		public void default_1()
		{
			RunCompilerTestCase(@"default-1.boo");
		}
		
		[Test]
		public void double_as_bool_1()
		{
			RunCompilerTestCase(@"double-as-bool-1.boo");
		}
		
		[Test]
		public void double_precision_is_used_for_literals()
		{
			RunCompilerTestCase(@"double-precision-is-used-for-literals.boo");
		}
		
		[Test]
		public void hash_1()
		{
			RunCompilerTestCase(@"hash-1.boo");
		}
		
		[Test]
		public void hex_1()
		{
			RunCompilerTestCase(@"hex-1.boo");
		}
		
		[Test]
		public void hex_2()
		{
			RunCompilerTestCase(@"hex-2.boo");
		}
		
		[Ignore("implicit casts for comparison operators still not implemented")][Test]
		public void implicit_casts_1()
		{
			RunCompilerTestCase(@"implicit-casts-1.boo");
		}
		
		[Test]
		public void int_shift_overflow_checked()
		{
			RunCompilerTestCase(@"int-shift-overflow-checked.boo");
		}
		
		[Test]
		public void int_shift_overflow_unchecked()
		{
			RunCompilerTestCase(@"int-shift-overflow-unchecked.boo");
		}
		
		[Test]
		public void interpolation_1()
		{
			RunCompilerTestCase(@"interpolation-1.boo");
		}
		
		[Test]
		public void len_1()
		{
			RunCompilerTestCase(@"len-1.boo");
		}
		
		[Test]
		public void list_1()
		{
			RunCompilerTestCase(@"list-1.boo");
		}
		
		[Test]
		public void list_2()
		{
			RunCompilerTestCase(@"list-2.boo");
		}
		
		[Test]
		public void list_3()
		{
			RunCompilerTestCase(@"list-3.boo");
		}
		
		[Test]
		public void long_1()
		{
			RunCompilerTestCase(@"long-1.boo");
		}
		
		[Test]
		public void primitives_1()
		{
			RunCompilerTestCase(@"primitives-1.boo");
		}
		
		[Test]
		public void promotion_1()
		{
			RunCompilerTestCase(@"promotion-1.boo");
		}
		
		[Test]
		public void promotion_2()
		{
			RunCompilerTestCase(@"promotion-2.boo");
		}
		
		[Test]
		public void regex_1()
		{
			RunCompilerTestCase(@"regex-1.boo");
		}
		
		[Test]
		public void single_as_bool_1()
		{
			RunCompilerTestCase(@"single-as-bool-1.boo");
		}
		
		[Test]
		public void string_1()
		{
			RunCompilerTestCase(@"string-1.boo");
		}
		
		[Test]
		public void string_yields_chars()
		{
			RunCompilerTestCase(@"string-yields-chars.boo");
		}
		
		[Test]
		public void typeof_1()
		{
			RunCompilerTestCase(@"typeof-1.boo");
		}
		
		[Test]
		public void typeof_2()
		{
			RunCompilerTestCase(@"typeof-2.boo");
		}
		
		[Test]
		public void uint_1()
		{
			RunCompilerTestCase(@"uint-1.boo");
		}
		
		[Test]
		public void uint_argument()
		{
			RunCompilerTestCase(@"uint-argument.boo");
		}
		
		[Test]
		public void uint_field_initializer()
		{
			RunCompilerTestCase(@"uint-field-initializer.boo");
		}
		
		[Test]
		public void ulong_bitshift()
		{
			RunCompilerTestCase(@"ulong-bitshift.boo");
		}
		
		[Test]
		public void unsigned_1()
		{
			RunCompilerTestCase(@"unsigned-1.boo");
		}
		
		[Test]
		public void unsigned_2()
		{
			RunCompilerTestCase(@"unsigned-2.boo");
		}
		
		[Test]
		public void __switch___1()
		{
			RunCompilerTestCase(@"__switch__-1.boo");
		}
		

		override protected string GetRelativeTestCasesPath()
		{
			return "integration/primitives";
		}
	}
}
