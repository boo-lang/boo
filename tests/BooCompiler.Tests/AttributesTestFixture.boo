namespace BooCompiler.Tests

import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps

[TestFixture]
partial class AttributesTestFixture(AbstractCompilerTestCase):
	override protected def SetUpCompilerPipeline() as CompilerPipeline:
		pipeline as CompilerPipeline = Boo.Lang.Compiler.Pipelines.ExpandMacros()
		pipeline.Add(PrintBoo())
		return pipeline
