namespace BooCompiler.Tests

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class BooCompilerTest:
	[Test]
	def EnvironmentBindingsCanBeCustomizedThroughCompilerParametersEnvironment():
		actualFormatter as EntityFormatter = null
		expectedFormatter = EntityFormatter()

		compiler = Boo.Lang.Compiler.BooCompiler()
		pipeline = CompilerPipeline()
		pipeline.Add(ActionStep({ actualFormatter = My[of EntityFormatter].Instance }))
		compiler.Parameters.Pipeline = pipeline
		compiler.Parameters.Environment = ClosedEnvironment(expectedFormatter)
		compiler.Run()

		Assert.AreSame(expectedFormatter, actualFormatter)
