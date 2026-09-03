namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class ClrExtensionsIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @clrextensions_1():
		RunCompilerTestCase("clrextensions-1.boo")

	[Test]
	def @clrextensions_2():
		RunCompilerTestCase("clrextensions-2.boo")

	[Test]
	def @infer_closure_singature():
		RunCompilerTestCase("infer-closure-singature.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/clr-extensions"
