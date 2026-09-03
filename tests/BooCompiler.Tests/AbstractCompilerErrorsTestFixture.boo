namespace BooCompiler.Tests

import Boo.Lang.Compiler

class AbstractCompilerErrorsTestFixture(AbstractCompilerTestCase):
	class PrintErrors(Boo.Lang.Compiler.Pipelines.Compile):
		protected override def OnAfter(context as CompilerContext):
			RunStep(context, Boo.Lang.Compiler.Steps.PrintErrors())

	protected override def SetUpCompilerPipeline() as CompilerPipeline:
		return PrintErrors()

	protected override IgnoreErrors as bool:
		get: return true
