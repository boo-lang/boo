namespace BooCompiler.Tests

import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines

[TestFixture]
partial class SemanticsTestFixture(AbstractCompilerTestCase):
	protected override def SetUpCompilerPipeline() as CompilerPipeline:
		return CompileToBoo()
