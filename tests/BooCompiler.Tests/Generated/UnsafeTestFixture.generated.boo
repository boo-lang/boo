namespace BooCompiler.Tests

import NUnit.Framework

partial class UnsafeTestFixture:

	[Test]
	def @sizeof_1():
		RunCompilerTestCase("sizeof-1.boo")

	[Test]
	def @unsafe_1():
		RunCompilerTestCase("unsafe-1.boo")

	[Test]
	def @unsafe_2():
		RunCompilerTestCase("unsafe-2.boo")

	[Test]
	def @unsafe_3():
		RunCompilerTestCase("unsafe-3.boo")

	[Test]
	def @unsafe_4():
		RunCompilerTestCase("unsafe-4.boo")

	[Test]
	def @unsafe_5():
		RunCompilerTestCase("unsafe-5.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "unsafe"
