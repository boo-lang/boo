namespace BooTemplate

import Boo.Lang.Compiler
		
class TemplateCompiler:

	[property(TemplateClassName, value is not null and len(value) > 0)]
	_className = "Template"
	
	[property(TemplateBaseClass, ValidateBaseClass(value))]
	_baseClass = AbstractTemplate
	
	def CompileFile([required] fname as string):
		return Compile(Boo.Lang.Compiler.IO.FileInput(fname))
	
	def Compile([required] input as ICompilerInput):
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(input)
		compiler.Parameters.OutputType = CompilerOutputType.Library
		
		pipeline = Pipelines.CompileToMemory()
		pipeline[0] = WSABoo.Parser.WSABooParsingStep()
		pipeline.Insert(0, TemplatePreProcessor())
		pipeline.InsertAfter(
				Steps.InitializeTypeSystemServices,
				ApplyTemplateSemantics(self))
		compiler.Parameters.Pipeline = pipeline
		
		return compiler.Run()
		
		
	private def ValidateBaseClass(type as System.Type):
		assert type is not null
		assert ITemplate in type.GetInterfaces()
		assert not type.IsSealed
		return true
