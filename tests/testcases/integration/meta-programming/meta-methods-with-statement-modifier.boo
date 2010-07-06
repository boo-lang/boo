"""
exception message: (x is not null)
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def assert_(condition as Expression):
	return [|
		raise $(condition.ToCodeString()) unless $condition 
	|]
	
module = [|
	import System
	
	x = null
	assert_ x is null
	assert_ x is not null
|]

try:
	compile(module, System.Reflection.Assembly.GetExecutingAssembly()).EntryPoint.Invoke(null, (null,))
except x:
	print "exception message:", x.InnerException.Message
