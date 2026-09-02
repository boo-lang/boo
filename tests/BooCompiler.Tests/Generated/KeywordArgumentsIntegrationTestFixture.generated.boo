namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class KeywordArgumentsIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @constructor_parameters():
		RunCompilerTestCase("constructor-parameters.boo")

	[Test]
	def @declares_nothing():
		RunCompilerTestCase("declares-nothing.boo")

	[Test]
	def @out_of_order():
		RunCompilerTestCase("out-of-order.boo")

	[Test]
	def @variable_as_value():
		RunCompilerTestCase("variable-as-value.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/keyword-arguments"
