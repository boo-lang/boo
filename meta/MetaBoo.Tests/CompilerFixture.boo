namespace MetaBoo.Tests

import System.IO
import NUnit.Framework
import MetaBoo
import MetaBoo.Pipelines
import MetaBoo.PipelineSteps

abstract class AbstractCompilerFixture:
	virtual def GetTestCasePath():
		return Path.GetDirectoryName(typeof(AbstractCompilerFixture).Assembly.Location)
		
	virtual def CreatePipeline() as CompilerPipeline:
		pass

[TestFixture]
class CompilerFixture(AbstractCompilerFixture):
	override def CreatePipeline():
		pipeline = CompileToFilePipeline()
		pipeline.Add(Run())
		return pipeline
		
	override def GetTestCasePath():
		return Path.Combine(super(), "compilation")
							
[TestFixture]
class MetaBooSemanticsTestCase(AbstractCompilerFixture):
	override def CreatePipeline():
		pipeline = CompilePipeline()
		pipeline.Add(PrintBoo())
		return pipeline
		
	override def GetTestCasePath():
		return Path.Combine(super(), "semantics")
