namespace HelloPipeline

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.Pipelines

class HelloPipelineStep(AbstractCompilerStep):
	
	override def Run():
		print("Hello from ${GetType()}!")
		

class HelloPipeline(CompileToFile):
	
	def constructor():
		self.Add(HelloPipelineStep())
