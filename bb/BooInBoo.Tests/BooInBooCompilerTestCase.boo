namespace BooInBoo.Tests

import System.IO
import NUnit.Framework
import BooInBoo
import BooInBoo.Pipelines
import BooInBoo.PipelineSteps

abstract class AbstractCompilerTestCase:
	virtual def GetTestCasePath():
		return Path.GetDirectoryName(typeof(AbstractCompilerTestCase).Assembly.Location)
		
	virtual def CreatePipeline() as CompilerPipeline:
		pass

[TestFixture]
class CompilerTestCase(AbstractCompilerTestCase):
	override def CreatePipeline():
		pipeline = CompileToFilePipeline()
		pipeline.Add(Run())
		return pipeline
		
	override def GetTestCasePath():
		return Path.Combine(super(), "compilation")
							
[TestFixture]
class BooInBooSemanticsTestCase(AbstractCompilerTestCase):
	override def CreatePipeline():
		pipeline = CompilePipeline()
		pipeline.Add(PrintBoo())
		return pipeline
		
	override def GetTestCasePath():
		return Path.Combine(super(), "semantics")
