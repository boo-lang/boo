"""
Hooray!
"""
import Boo.Lang.Compiler.MetaProgramming
import Boo.Lang.Compiler.Ast

module1 = [|
	namespace Boo.Lang.Extensions
	
	macro assert:
		yield [| print "Hooray!" |]
|]
module2 = [|
	import Boo.Lang.Extensions
	
	assert false
|]
compile(CompileUnit(module1, module2)).EntryPoint.Invoke(null, (null,))
