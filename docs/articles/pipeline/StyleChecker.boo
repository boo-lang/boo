namespace Boo.Docs.Articles.Pipeline

import Boo.Lang.Compiler

class StyleChecker(AbstractCompilerStep):
	
	override def Run():
		print("I'm running!")
		

class StyleCheckingPipelineDefinition(ICompilerPipelineDefinition):
	
	def SetUp(pipeline as CompilerPipeline):
		pipeline.Load("booc")
		pipeline.InsertAfter("parse", StyleChecker())
