namespace BooCompiler.Tests

import NUnit.Framework
import Boo.Lang.Compiler

[TestFixture]
partial class CompilerWarningsTestFixture(AbstractCompilerTestCase):
	protected override def SetUpCompilerPipeline() as CompilerPipeline:
		pipeline as CompilerPipeline = Boo.Lang.Compiler.Pipelines.Compile()
		pipeline.Add(Boo.Lang.Compiler.Steps.PrintWarnings())
		return pipeline
