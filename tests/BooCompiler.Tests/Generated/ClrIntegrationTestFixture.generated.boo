namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class ClrIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @RealProxy_1():
		RunCompilerTestCase("RealProxy-1.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/clr"
