namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class DuckTypingIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @duck_1():
		RunCompilerTestCase("duck-1.boo")

	[Test]
	def @duck_10():
		RunCompilerTestCase("duck-10.boo")

	[Test]
	def @duck_11():
		RunCompilerTestCase("duck-11.boo")

	[Test]
	def @duck_12():
		RunCompilerTestCase("duck-12.boo")

	[Test]
	def @duck_13():
		RunCompilerTestCase("duck-13.boo")

	[Test]
	def @duck_14():
		RunCompilerTestCase("duck-14.boo")

	[Test]
	def @duck_15():
		RunCompilerTestCase("duck-15.boo")

	[Test]
	def @duck_16():
		RunCompilerTestCase("duck-16.boo")

	[Test]
	def @duck_17():
		RunCompilerTestCase("duck-17.boo")

	[Test]
	def @duck_18():
		RunCompilerTestCase("duck-18.boo")

	[Test]
	def @duck_19():
		RunCompilerTestCase("duck-19.boo")

	[Test]
	def @duck_2():
		RunCompilerTestCase("duck-2.boo")

	[Test]
	def @duck_20():
		RunCompilerTestCase("duck-20.boo")

	[Test]
	def @duck_21():
		RunCompilerTestCase("duck-21.boo")

	[Test]
	def @duck_3():
		RunCompilerTestCase("duck-3.boo")

	[Test]
	def @duck_4():
		RunCompilerTestCase("duck-4.boo")

	[Test]
	def @duck_5():
		RunCompilerTestCase("duck-5.boo")

	[Test]
	def @duck_6():
		RunCompilerTestCase("duck-6.boo")

	[Test]
	def @duck_7():
		RunCompilerTestCase("duck-7.boo")

	[Test]
	def @duck_8():
		RunCompilerTestCase("duck-8.boo")

	[Test]
	def @duck_9():
		RunCompilerTestCase("duck-9.boo")

	[Test]
	def @exceptions_1():
		RunCompilerTestCase("exceptions-1.boo")

	[Test]
	def @exceptions_2():
		RunCompilerTestCase("exceptions-2.boo")

	[Test]
	def @exceptions_3():
		RunCompilerTestCase("exceptions-3.boo")

	[Test]
	def @exceptions_4():
		RunCompilerTestCase("exceptions-4.boo")

	[Test]
	def @exceptions_5():
		RunCompilerTestCase("exceptions-5.boo")

	[Test]
	def @exceptions_6():
		RunCompilerTestCase("exceptions-6.boo")

	[Test]
	def @exceptions_7():
		RunCompilerTestCase("exceptions-7.boo")

	[Test]
	def @exceptions_8():
		RunCompilerTestCase("exceptions-8.boo")

	[Test]
	def @exceptions_9():
		RunCompilerTestCase("exceptions-9.boo")

	[Test]
	def @indexer_1():
		RunCompilerTestCase("indexer-1.boo")

	[Test]
	def @promotion_1():
		RunCompilerTestCase("promotion-1.boo")

	[Test]
	def @promotion_2():
		RunCompilerTestCase("promotion-2.boo")

	[Test]
	def @promotion_3():
		RunCompilerTestCase("promotion-3.boo")

	[Test]
	def @promotion_4():
		RunCompilerTestCase("promotion-4.boo")

	[Test]
	def @promotion_5():
		RunCompilerTestCase("promotion-5.boo")

	[Test]
	def @runtime_extensions_1():
		RunCompilerTestCase("runtime-extensions-1.boo")

	[Test]
	def @runtime_extensions_2():
		RunCompilerTestCase("runtime-extensions-2.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/duck-typing"
