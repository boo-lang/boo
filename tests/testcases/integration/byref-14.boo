"""
42
"""
import Boo.Lang.Compiler
import System.Reflection

def compile(name as string, code as string, *references as (Assembly)):
	compiler = BooCompiler()
	compiler.Parameters.Input.Add(IO.StringInput(name, code))
	compiler.Parameters.Pipeline = Pipelines.CompileToMemory()
	compiler.Parameters.References.Extend(references)
	result = compiler.Run()
	assert 0 == len(result.Errors), result.Errors.ToString(true)
	return result.GeneratedAssembly

def foo(ref i as int):
	i = 42

code = """
f = foo
i = 0
f(i)
print i
"""

assembly = compile("code", code, Assembly.GetExecutingAssembly())
assembly.EntryPoint.Invoke(null, (null,))
