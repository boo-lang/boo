namespace BooInBoo.Pipelines

import BooInBoo

class ParsePipeline(CompilerPipeline):
	static _defaultParserStepType = System.Type.GetType("Boo.AntlrParser.BooParsingStep, Boo.AntlrParser", true)
		
	override def Initialize():
		Add(_defaultParserStepType())

class CompilePipeline(ParsePipeline):
	override def Initialize():
		pass
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

class CompileToFilePipeline(CompilePipeline):

	override def Initialize():
		pass
