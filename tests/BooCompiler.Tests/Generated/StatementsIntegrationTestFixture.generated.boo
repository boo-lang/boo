namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class StatementsIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @break_1():
		RunCompilerTestCase("break-1.boo")

	[Test]
	def @break_2():
		RunCompilerTestCase("break-2.boo")

	[Test]
	def @continue_1():
		RunCompilerTestCase("continue-1.boo")

	[Test]
	def @continue_2():
		RunCompilerTestCase("continue-2.boo")

	[Test]
	def @declaration_1():
		RunCompilerTestCase("declaration-1.boo")

	[Test]
	def @declaration_2():
		RunCompilerTestCase("declaration-2.boo")

	[Test]
	def @declaration_3():
		RunCompilerTestCase("declaration-3.boo")

	[Test]
	def @except_1():
		RunCompilerTestCase("except-1.boo")

	[Test]
	def @except_10():
		RunCompilerTestCase("except-10.boo")

	[Test]
	def @except_11():
		RunCompilerTestCase("except-11.boo")

	[Test]
	def @except_12():
		RunCompilerTestCase("except-12.boo")

	[Test]
	def @except_13():
		RunCompilerTestCase("except-13.boo")

	[Test]
	def @except_14():
		RunCompilerTestCase("except-14.boo")

	[Test]
	def @except_2():
		RunCompilerTestCase("except-2.boo")

	[Test]
	def @except_3():
		RunCompilerTestCase("except-3.boo")

	[Test]
	def @except_4():
		RunCompilerTestCase("except-4.boo")

	[Test]
	def @except_5():
		RunCompilerTestCase("except-5.boo")

	[Test]
	def @except_6():
		RunCompilerTestCase("except-6.boo")

	[Test]
	def @except_7():
		RunCompilerTestCase("except-7.boo")

	[Test]
	def @except_8():
		RunCompilerTestCase("except-8.boo")

	[Test]
	def @except_9():
		RunCompilerTestCase("except-9.boo")

	[Test]
	def @failure_1():
		RunCompilerTestCase("failure-1.boo")

	[Test]
	def @failure_2():
		RunCompilerTestCase("failure-2.boo")

	[Test]
	def @failure_3():
		RunCompilerTestCase("failure-3.boo")

	[Test]
	def @failure_4():
		RunCompilerTestCase("failure-4.boo")

	[Test]
	def @failure_5():
		RunCompilerTestCase("failure-5.boo")

	[Test]
	def @failure_6():
		RunCompilerTestCase("failure-6.boo")

	[Test]
	def @filter_1():
		RunCompilerTestCase("filter-1.boo")

	[Test]
	def @filter_2():
		RunCompilerTestCase("filter-2.boo")

	[Test]
	def @filter_3():
		RunCompilerTestCase("filter-3.boo")

	[Test]
	def @for_1():
		RunCompilerTestCase("for-1.boo")

	[Test]
	def @for_10():
		RunCompilerTestCase("for-10.boo")

	[Test]
	def @for_2():
		RunCompilerTestCase("for-2.boo")

	[Test]
	def @for_3():
		RunCompilerTestCase("for-3.boo")

	[Test]
	def @for_4():
		RunCompilerTestCase("for-4.boo")

	[Test]
	def @for_5():
		RunCompilerTestCase("for-5.boo")

	[Test]
	def @for_6():
		RunCompilerTestCase("for-6.boo")

	[Test]
	def @for_6B():
		RunCompilerTestCase("for-6B.boo")

	[Test]
	def @for_7():
		RunCompilerTestCase("for-7.boo")

	[Test]
	def @for_8():
		RunCompilerTestCase("for-8.boo")

	[Test]
	def @for_9():
		RunCompilerTestCase("for-9.boo")

	[Test]
	def @for_array_1():
		RunCompilerTestCase("for-array-1.boo")

	[Test]
	def @for_array_2():
		RunCompilerTestCase("for-array-2.boo")

	[Test]
	def @for_var_reuse():
		RunCompilerTestCase("for-var-reuse.boo")

	[Test]
	def @for_or_1():
		RunCompilerTestCase("for_or-1.boo")

	[Test]
	def @for_or_2():
		RunCompilerTestCase("for_or-2.boo")

	[Test]
	def @for_or_3():
		RunCompilerTestCase("for_or-3.boo")

	[Test]
	def @for_or_4():
		RunCompilerTestCase("for_or-4.boo")

	[Test]
	def @for_or_then_1():
		RunCompilerTestCase("for_or_then-1.boo")

	[Test]
	def @for_or_then_3():
		RunCompilerTestCase("for_or_then-3.boo")

	[Test]
	def @for_or_then_4():
		RunCompilerTestCase("for_or_then-4.boo")

	[Test]
	def @for_or_then_5():
		RunCompilerTestCase("for_or_then-5.boo")

	[Test]
	def @for_then_1():
		RunCompilerTestCase("for_then-1.boo")

	[Test]
	def @for_then_2():
		RunCompilerTestCase("for_then-2.boo")

	[Test]
	def @for_then_3():
		RunCompilerTestCase("for_then-3.boo")

	[Test]
	def @for_then_4():
		RunCompilerTestCase("for_then-4.boo")

	[Test]
	def @goto_1():
		RunCompilerTestCase("goto-1.boo")

	[Test]
	def @goto_2():
		RunCompilerTestCase("goto-2.boo")

	[Test]
	def @goto_3():
		RunCompilerTestCase("goto-3.boo")

	[Test]
	def @goto_4():
		RunCompilerTestCase("goto-4.boo")

	[Test]
	def @goto_5():
		RunCompilerTestCase("goto-5.boo")

	[Test]
	def @goto_6():
		RunCompilerTestCase("goto-6.boo")

	[Test]
	def @raise_1():
		RunCompilerTestCase("raise-1.boo")

	[Test]
	def @reraise_1():
		RunCompilerTestCase("reraise-1.boo")

	[Test]
	def @reraise_2():
		RunCompilerTestCase("reraise-2.boo")

	[Test]
	def @try_1():
		RunCompilerTestCase("try-1.boo")

	[Test]
	def @try_2():
		RunCompilerTestCase("try-2.boo")

	[Test]
	def @try_3():
		RunCompilerTestCase("try-3.boo")

	[Test]
	def @unpack_1():
		RunCompilerTestCase("unpack-1.boo")

	[Test]
	def @unpack_10():
		RunCompilerTestCase("unpack-10.boo")

	[Test]
	def @unpack_11():
		RunCompilerTestCase("unpack-11.boo")

	[Test]
	def @unpack_12():
		RunCompilerTestCase("unpack-12.boo")

	[Test]
	def @unpack_13():
		RunCompilerTestCase("unpack-13.boo")

	[Test]
	def @unpack_2():
		RunCompilerTestCase("unpack-2.boo")

	[Test]
	def @unpack_3():
		RunCompilerTestCase("unpack-3.boo")

	[Test]
	def @unpack_4():
		RunCompilerTestCase("unpack-4.boo")

	[Test]
	def @unpack_5():
		RunCompilerTestCase("unpack-5.boo")

	[Test]
	def @unpack_6():
		RunCompilerTestCase("unpack-6.boo")

	[Test]
	def @unpack_7():
		RunCompilerTestCase("unpack-7.boo")

	[Test]
	def @unpack_8():
		RunCompilerTestCase("unpack-8.boo")

	[Test]
	def @unpack_9():
		RunCompilerTestCase("unpack-9.boo")

	[Test]
	def @while_1():
		RunCompilerTestCase("while-1.boo")

	[Test]
	def @while_2():
		RunCompilerTestCase("while-2.boo")

	[Test]
	def @while_3():
		RunCompilerTestCase("while-3.boo")

	[Test]
	def @while_4():
		RunCompilerTestCase("while-4.boo")

	[Test]
	def @while_5():
		RunCompilerTestCase("while-5.boo")

	[Test]
	def @while_6():
		RunCompilerTestCase("while-6.boo")

	[Test]
	def @while_7():
		RunCompilerTestCase("while-7.boo")

	[Test]
	def @while_8():
		RunCompilerTestCase("while-8.boo")

	[Test]
	def @while_or_1():
		RunCompilerTestCase("while_or-1.boo")

	[Test]
	def @while_or_2():
		RunCompilerTestCase("while_or-2.boo")

	[Test]
	def @while_or_3():
		RunCompilerTestCase("while_or-3.boo")

	[Test]
	def @while_or_4():
		RunCompilerTestCase("while_or-4.boo")

	[Test]
	def @while_or_then_1():
		RunCompilerTestCase("while_or_then-1.boo")

	[Test]
	def @while_or_then_4():
		RunCompilerTestCase("while_or_then-4.boo")

	[Test]
	def @while_or_then_5():
		RunCompilerTestCase("while_or_then-5.boo")

	[Test]
	def @while_then_1():
		RunCompilerTestCase("while_then-1.boo")

	[Test]
	def @while_then_2():
		RunCompilerTestCase("while_then-2.boo")

	[Test]
	def @while_then_3():
		RunCompilerTestCase("while_then-3.boo")

	[Test]
	def @while_then_4():
		RunCompilerTestCase("while_then-4.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/statements"
