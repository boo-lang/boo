namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class LinqIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @array_linq():
		RunCompilerTestCase("array-linq.boo")

	[Test]
	def @linq_aggregate():
		RunCompilerTestCase("linq-aggregate.boo")

	[Test]
	def @linq_extensions_1():
		RunCompilerTestCase("linq-extensions-1.boo")

	[Test]
	def @linq_extensions_2():
		RunCompilerTestCase("linq-extensions-2.boo")

	[Test]
	def @overload_resolution_extension():
		RunCompilerTestCase("overload-resolution-extension.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/linq"
