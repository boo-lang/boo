namespace BooExplorer

import System.Text.RegularExpressions
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

class CompiledScript:
	_ctx as CompilerContext
	
	Errors as CompilerErrorCollection:
		get:
			return _ctx.Errors

	def constructor(ctx as CompilerContext):
		_ctx = ctx
				
	def Execute():
		return if len(_ctx.Errors)
		_ctx.GeneratedAssemblyEntryPoint.Invoke(null, (null,))
		
	def GetType(typeName as string):
		return null if len(_ctx.Errors)
		return _ctx.GeneratedAssembly.GetType(typeName)
	
	def GetTypes(match as string):
		return null if len(_ctx.Errors)
		return [t for t in _ctx.GeneratedAssembly.GetTypes() if t.Name =~ Regex(match)]
		
	
	def GetTypes():
		return null if len(_ctx.Errors)
		return _ctx.GeneratedAssembly.GetTypes()

class ScriptCompiler:
	static def CompileFile([required] fileName as string) as CompiledScript:
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(FileInput(fileName))
		compiler.Parameters.Pipeline = CompileToMemory()
		compiler.Parameters.References.Add(System.Reflection.Assembly.GetExecutingAssembly())
		compiler.Parameters.OutputType = CompilerOutputType.Library
		return CompiledScript(compiler.Run())
