namespace MetaBoo.Pipelines

import MetaBoo

class ParsePipeline(CompilerPipeline):
	static _defaultParserStepType = System.Type.GetType("Boo.AntlrParser.BooParsingStep, Boo.AntlrParser", true)
		
	def constructor():
		Add(_defaultParserStepType())

class CompilePipeline(ParsePipeline):
	def constructor():
		pass
		/*		
		pipeline.Add(IntroduceBindingService())
		pipeline.Add(BindNamespaces())
		pipeline.Add(IntroduceNameResolutionService())
		pipeline.Add(IntroduceGlobalNamespaces())
		pipeline.Add(BindAndApplyAttributes())
		pipeline.Add(ExpandMacros())
		pipeline.Add(IntroduceModuleClasses())
		pipeline.Add(NormalizeTypeMembers())
		pipeline.Add(NormalizeStatementModifiers())
		pipeline.Add(BindTypeDefinitions())
		pipeline.Add(BindTypeMembers())
		pipeline.Add(CheckTypeMemberDeclarations())
		pipeline.Add(PreOptimizeExpressions())
		pipeline.Add(IntroduceCallableResolutionService())
		pipeline.Add(ProcessMethodBodies())
		pipeline.Add(ProcessGenerators()) // for and yield
		pipeline.Add(CheckInterfaceImplementations())
		pipeline.Add(InjectCasts())
		*/

class CompileToFilePipeline(CompilePipeline):

	def constructor():
		pass
