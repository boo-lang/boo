namespace BooInBoo.PipelineDefinitions

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipeline

class Compile(ICompilerPipelineDefinition):
	static _defaultParserStepType = System.Type.GetType("Boo.AntlrParser.BooParsingStep, Boo.AntlrParser", true)
	
	virtual def Define(pipeline as CompilerPipeline):
		pipeline.Add(_defaultParserStepType())

class CompileToFile(Compile):

	override def Define(pipeline as CompilerPipeline):
		pass
