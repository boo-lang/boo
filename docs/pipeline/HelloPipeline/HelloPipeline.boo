namespace HelloPipeline

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipeline.Definitions

class HelloPipelineStep(AbstractCompilerStep):
	
	override def Run():
		print("Hello from ${GetType()}!")
		

class HelloPipelineDefinition(BoocPipelineDefinition):
	
	override def Define(pipeline as CompilerPipeline):
		super(pipeline)
		pipeline.Add(HelloPipelineStep())
