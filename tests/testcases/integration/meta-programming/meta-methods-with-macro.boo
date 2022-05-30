"""
exception message: false
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def assert_(condition as Expression):
	return [| assert $condition |]
	
module = [|
	import System
	
	assert_ false
|]

try:
	compile(module, System.Reflection.Assembly.GetExecutingAssembly()).GetEntryPoint().Invoke(null, (null,))
except x:
	print "exception message:", (x.InnerException or x).Message
