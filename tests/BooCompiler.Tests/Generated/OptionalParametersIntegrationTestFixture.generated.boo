namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
class OptionalParametersIntegrationTestFixture(AbstractCompilerTestCase):

	[Test]
	def @corelib_omitted_argument():
		RunCompilerTestCase("corelib-omitted-argument.boo")

	[Test]
	def @declared_defaults():
		RunCompilerTestCase("declared-defaults.boo")

	[Test]
	def @enum_default_reaches_callee():
		RunCompilerTestCase("enum-default-reaches-callee.boo")

	[Test]
	def @exact_overload_preferred():
		RunCompilerTestCase("exact-overload-preferred.boo")

	[Test]
	def @omitted_argument_still_runs():
		RunCompilerTestCase("omitted-argument-still-runs.boo")

	[Test]
	def @partially_supplied_defaults():
		RunCompilerTestCase("partially-supplied-defaults.boo")

	override protected def GetRelativeTestCasesPath() as string:
		return "integration/optional-parameters"
