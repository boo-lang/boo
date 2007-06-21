"""
IEnumerable.Each
1
2
3
"""

// this test makes sure extension methods are recognized in compiled
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
namespace MyExtensions

[Extension]
def Each(e as System.Collections.IEnumerable, action as callable(object)):
	print 'IEnumerable.Each'
	for item in e:
		action(item)
"""

library = compile(code, [])

code = """
import MyExtensions

class App:
	static def Main():
		(1, 2, 3).Each(print)
"""

app = compile(code, [library])
(app.GetType("App") as duck).Main()

	

