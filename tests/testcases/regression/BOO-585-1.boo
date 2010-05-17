"""
abc
 def
_allo
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines

module1 = """
def Main(argv as (string)):
    Toto.Initialize()
"""

module2 = """
class Toto:
	static def Initialize():
		_toto = def(s as string):
			refs = s.Split(char(','))
			for d in refs:
				print d
		_toto("abc, def")
			
		_toto2 = def(s as string):
			return '_' + s[0:1].ToLower() + s[1:]
		print _toto2("Allo")
"""

compiler = BooCompiler()
compiler.Parameters.Input.Add(StringInput("module1", module1))
compiler.Parameters.Input.Add(StringInput("module2", module2))
compiler.Parameters.Pipeline = Run()
context = compiler.Run()
assert len(context.Errors) == 0, context.Errors.ToString()


