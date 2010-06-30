"""
compile time: (x is not null)
compile time: (x is not null) 'custom message'
before runtime
OneArg
exception message: (x is not null)
TwoArgs
exception message: custom message
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def assert_(condition as Expression):
	print "compile time:", condition.ToCodeString()
	return [|
		if not $condition: raise $(condition.ToCodeString())
	|]

[meta]
def assert_(condition as Expression, exception as Expression):
	print "compile time:", condition.ToCodeString(), exception.ToCodeString()
	return [|
		if not $condition: raise $exception
	|]

def captureException(block as callable()):
	try:
		block()
	except x:
		print "exception message:", (x.InnerException or x).Message


typeDef = [|
	class Test:
		def OneArg():
			print "OneArg"
			x = null
			assert_ x is not null

		def TwoArgs():
			print "TwoArgs"
			x = null
			assert_ x is not null, "custom message"
|]

type = compile(typeDef, System.Reflection.Assembly.GetExecutingAssembly())
print "before runtime"

test = type() as duck
captureException:
	test.OneArg()
captureException:
	test.TwoArgs()

