namespace BooInBoo.PipelineDefinitions

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipeline

class Compile(ICompilerPipelineDefinition):
	static _defaultParserStepType = System.Type.GetType("Boo.AntlrParser.BooParsingStep, Boo.AntlrParser", true)
	
	def Define(pipeline as CompilerPipeline):
		pipeline.Add(_defaultParserStepType())
		/*
		pipeline.Add(IntroduceGlobalNamespaces())
		pipeline.Add(IntroduceBindingService())
		pipeline.Add(BindNamespaces())
		pipeline.Add(IntroduceNameResolutionService())
		pipeline.Add(BindAndApplyAttributes())
		pipeline.Add(ExpandMacros())
		pipeline.Add(IntroduceModuleClasses())
		pipeline.Add(NormalizeVisibility())
		pipeline.Add(NormalizeStatementModifiers())
		pipeline.Add(BindTypeDefinitions())
		pipeline.Add(BindTypeMembers())
		pipeline.Add(CheckTypeMemberDeclarations())
		pipeline.Add(PreOptimizeExpressions())
		pipeline.Add(IntroduceCallableResolutionService())
		pipeline.Add(ProcessMethodBodies())
		pipeline.Add(CheckInterfaceImplementations())
		*/

class CompileToFile(Compile):

	override def Define(pipeline as CompilerPipeline):
		pass
