"""
Bill
Bill
"""

// this test makes sure duck types are preserved and recognized in compiled
// assemblies

import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

def compile(code, references):
	compiler = BooCompiler()
	for reference in references:
		compiler.Parameters.References.Add(reference)
	compiler.Parameters.Input.Add(StringInput("code", code))
	compiler.Parameters.Pipeline = CompileToMemory()
	compiler.Parameters.OutputType = CompilerOutputType.Library
	result = compiler.Run()
	assert 0 == len(result.Errors), result.Errors.ToString()
	return result.GeneratedAssembly
	
code = """
class Person:
	[property(Name)]
	_name = ''
	
class Foo:
	static def bar() as duck:
		return Person(Name: 'Bill')
		
	static baz as duck:
		get:
			return Person(Name: 'Bill')
"""

library = compile(code, [])

code = """
class App:
	def Run():
		print bar().Name
		print baz().Name
		
	def bar():
		return Foo.bar()
		
	def baz():
		return Foo.baz
"""

app = compile(code, [library])
(app.GetType("App")() as duck).Run()

	

