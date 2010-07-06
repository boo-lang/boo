"""
1
3
5
7
9
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def select(list as Expression, condition as Expression):
	return [| (it for it in $list if $condition) |]
	
module = [|
	import System
	
	for item in select(range(10), it % 2):
		print item
|]

compile(module, System.Reflection.Assembly.GetExecutingAssembly()).EntryPoint.Invoke(null, (null,))
