"""
<code>(8,34): BCE0023: No appropriate version of 'CompilerGeneratedExtensions.BeginInvoke' for the argument list '(callable(string) as int)' was found.
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps

def compile(code as string):	
	compiler = BooCompiler()
	compiler.Parameters.OutputType = CompilerOutputType.Library
	compiler.Parameters.Input.Add(StringInput("<code>", code))
	compiler.Parameters.Pipeline = CompileToMemory()
	return compiler.Run()

code = """	
class Foo:
	def Bar1(parameter as string):
		print "Something"
		return 5

	def Bar2():
		result = Bar1.BeginInvoke()
"""

print compile(code).Errors.ToString()
