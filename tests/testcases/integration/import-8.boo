import Boo.Lang.Compiler

def compile(src, references):
	compiler = BooCompiler()
	compiler.Parameters.References.Extend(references)
	compiler.Parameters.Input.Add(src)
	compiler.Parameters.Pipeline = Pipelines.CompileToMemory()
	compiler.Parameters.OutputType = CompilerOutputType.Library
	result = compiler.Run()
	assert 0 == len(result.Errors), result.Errors.ToString(true)
	return result.GeneratedAssembly
	

librarySource = """
namespace CustomLibrary

def foo():
	return "foo"
"""

lib = compile(IO.StringInput("CustomAssembly", librarySource), [])
assert lib is not null
assert lib.FullName.StartsWith("CustomAssembly")

clientSource = """
import CustomLibrary from CustomAssembly

assert 'foo' == foo()
"""
compile(IO.StringInput("client", clientSource), [lib])





