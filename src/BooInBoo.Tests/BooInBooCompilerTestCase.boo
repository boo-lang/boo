namespace BooInBoo.Tests

import System.IO
import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Tests
import BooInBoo.PipelineDefinitions

[TestFixture]
class BooInBooCompilerTestCase(CompilerTestCase):
	override def SetUpCompilerPipeline(pipeline as CompilerPipeline):
		pipeline.Load(CompileToFile)
		pipeline.Add(BooInBoo.CompilerSteps.Run())
		
	override def GetTestCasePath(fname as string):
		return Path.Combine(Path.GetFullPath(super("../../../testcases/compilation")),
							fname)
							
[TestFixture]
class BooInBooSemanticsTestCase(SemanticsTestCase):
	override def SetUpCompilerPipeline(pipeline as CompilerPipeline):
		pipeline.Load(Compile)
		pipeline.Add(BooInBoo.CompilerSteps.PrintBoo())
		
	override def GetTestCasePath(fname as string):
		return Path.Combine(Path.GetFullPath(super("../../../testcases/semantics")),
							fname)
