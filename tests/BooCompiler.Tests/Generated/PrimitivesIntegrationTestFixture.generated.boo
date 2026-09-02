namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class PrimitivesIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @__switch___1():
		RunCompilerTestCase("__switch__-1.boo")

	[Test]
	def @at_operator():
		RunCompilerTestCase("at-operator.boo")

	[Test]
	def @bool_1():
		RunCompilerTestCase("bool-1.boo")

	[Test]
	def @char_1():
		RunCompilerTestCase("char-1.boo")

	[Test]
	def @char_2():
		RunCompilerTestCase("char-2.boo")

	[Test]
	def @char_3():
		RunCompilerTestCase("char-3.boo")

	[Test]
	def @char_4():
		RunCompilerTestCase("char-4.boo")

	[Test]
	def @char_5():
		RunCompilerTestCase("char-5.boo")

	[Test]
	def @checked_1():
		RunCompilerTestCase("checked-1.boo")

	[Test]
	def @decimal_1():
		RunCompilerTestCase("decimal-1.boo")

	[Test]
	def @default_1():
		RunCompilerTestCase("default-1.boo")

	[Test]
	def @double_as_bool_1():
		RunCompilerTestCase("double-as-bool-1.boo")

	[Test]
	def @double_precision_is_used_for_literals():
		RunCompilerTestCase("double-precision-is-used-for-literals.boo")

	[Test]
	def @hash_1():
		RunCompilerTestCase("hash-1.boo")

	[Test]
	def @hex_1():
		RunCompilerTestCase("hex-1.boo")

	[Test]
	def @hex_2():
		RunCompilerTestCase("hex-2.boo")

	[Ignore("implicit casts for comparison operators still not implemented")][Test]
	def @implicit_casts_1():
		RunCompilerTestCase("implicit-casts-1.boo")

	[Test]
	def @int_shift_overflow_checked():
		RunCompilerTestCase("int-shift-overflow-checked.boo")

	[Test]
	def @int_shift_overflow_unchecked():
		RunCompilerTestCase("int-shift-overflow-unchecked.boo")

	[Test]
	def @interpolation_1():
		RunCompilerTestCase("interpolation-1.boo")

	[Test]
	def @len_1():
		RunCompilerTestCase("len-1.boo")

	[Test]
	def @list_1():
		RunCompilerTestCase("list-1.boo")

	[Test]
	def @list_2():
		RunCompilerTestCase("list-2.boo")

	[Test]
	def @list_3():
		RunCompilerTestCase("list-3.boo")

	[Test]
	def @long_1():
		RunCompilerTestCase("long-1.boo")

	[Test]
	def @long_truth_1():
		RunCompilerTestCase("long-truth-1.boo")

	[Test]
	def @primitives_1():
		RunCompilerTestCase("primitives-1.boo")

	[Test]
	def @promotion_1():
		RunCompilerTestCase("promotion-1.boo")

	[Test]
	def @promotion_2():
		RunCompilerTestCase("promotion-2.boo")

	[Test]
	def @regex_1():
		RunCompilerTestCase("regex-1.boo")

	[Test]
	def @single_as_bool_1():
		RunCompilerTestCase("single-as-bool-1.boo")

	[Test]
	def @string_1():
		RunCompilerTestCase("string-1.boo")

	[Test]
	def @string_yields_chars():
		RunCompilerTestCase("string-yields-chars.boo")

	[Test]
	def @typeof_1():
		RunCompilerTestCase("typeof-1.boo")

	[Test]
	def @typeof_2():
		RunCompilerTestCase("typeof-2.boo")

	[Test]
	def @uint_1():
		RunCompilerTestCase("uint-1.boo")

	[Test]
	def @uint_argument():
		RunCompilerTestCase("uint-argument.boo")

	[Test]
	def @uint_field_initializer():
		RunCompilerTestCase("uint-field-initializer.boo")

	[Test]
	def @ulong_bitshift():
		RunCompilerTestCase("ulong-bitshift.boo")

	[Test]
	def @unsigned_1():
		RunCompilerTestCase("unsigned-1.boo")

	[Test]
	def @unsigned_2():
		RunCompilerTestCase("unsigned-2.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/primitives"
