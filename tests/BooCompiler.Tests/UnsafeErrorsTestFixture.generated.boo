namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class UnsafeErrorsTestFixture(AbstractCompilerErrorsTestFixture):
	protected override def CustomizeCompilerParameters():
		_parameters.Unsafe = true

	[Test]
	def @BCE0168_1():
		RunCompilerTestCase("BCE0168-1.boo")

	[Test]
	def @sizeof_usage_1():
		RunCompilerTestCase("sizeof-usage-1.boo")

	[Test]
	def @unsafe_usage_1():
		RunCompilerTestCase("unsafe-usage-1.boo")

	[Test]
	def @unsafe_usage_2():
		RunCompilerTestCase("unsafe-usage-2.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "unsafe/errors"
