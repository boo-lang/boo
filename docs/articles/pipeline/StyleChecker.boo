namespace Boo.Docs.Articles.Pipeline

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipeline.Definitions

class StyleChecker(AbstractCompilerStep):
	
	override def Run():
		print("I'm running!")
		

class StyleCheckingPipelineDefinition(BoocPipelineDefinition):
	
	override def Define(pipeline as CompilerPipeline):
		super(pipeline)
		pipeline.InsertAfter("parse", StyleChecker())
