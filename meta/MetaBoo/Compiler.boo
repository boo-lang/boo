namespace MetaBoo

import System

interface ICompilerComponent(System.IDisposable):
	def Initialize(context as CompilerContext)

interface ICompilerPipelineStep(ICompilerComponent):
	def Run()
	
interface ICompilerInput:
	Name as string:
		get
		
	def Open() as System.IO.TextReader

interface ICompilerResource:
	Name as string:
		get
	Description as string:
		get
	def WriteResources(writer as System.Resources.IResourceWriter)
	
class AbstractCompilerPipelineStep(ICompilerPipelineStep):
	_context as CompilerContext
	
	def Initialize(context as CompilerContext):
		_context = context
		
	def Dispose():
		_context = null
		
	CompileUnit:
		get:
			return _context.CompileUnit

class CompilerPipeline:
	_steps = []
	
	def Add([required] step as ICompilerPipelineStep):
		pass
		
class CompilerContext:
	CompileUnit as Boo.Lang.Compiler.Ast.CompileUnit:
		get:
			return null

class Compiler:
	pass
