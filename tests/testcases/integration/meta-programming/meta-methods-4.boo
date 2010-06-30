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
def assert_(*args as (Expression)):
"""
meta methods can also declare a variable number of arguments.
"""
	if len(args) > 1:
		condition, exception = args
		print "compile time:", condition, exception
	else:
		condition, exception = args[0], Expression.Lift(args[0].ToCodeString())
		print "compile time:", condition

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

