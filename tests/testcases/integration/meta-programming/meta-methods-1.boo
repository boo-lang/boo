"""
compile time: (x is null)
compile time: (x is not null)
before runtime
runtime
exception message: (x is not null)
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def assert_(condition as Expression):
	print "compile time:", condition.ToCodeString()
	return [|
		if not $condition: raise $(condition.ToCodeString())
	|]
	
typeDef = [|
	class Test:
		def Run():
			print "runtime"
			x = null
			assert_ x is null
			assert_ x is not null
|]

type = compile(typeDef, System.Reflection.Assembly.GetExecutingAssembly())
print "before runtime"
try:
	(type() as duck).Run()
except x:
	print "exception message:", (x.InnerException or x).Message
