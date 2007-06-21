"""
compile time: (x is not null)
before runtime
runtime
exception message: (x is not null)
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def assert_(e as Expression):
	print "compile time: ${e.ToCodeString()}"
	return [|
		if not $e: raise $(e.ToCodeString())
	|]

code = [|
	class Test:
		def Run():
			print "runtime"
			x = null
			assert_ x is not null
|]

type = compile(code, System.Reflection.Assembly.GetExecutingAssembly())
print "before runtime"
try:
	(type() as duck).Run()
except x:
	print "exception message:", x.Message
