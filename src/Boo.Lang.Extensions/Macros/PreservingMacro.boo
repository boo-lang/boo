namespace Boo.Lang.Extensions

import Boo.Lang.Compiler.Ast

macro preserving:
"""
Saves a group of values before executing a block and restores them after the block executes.

foo = 0
bar = [true]
preserving foo, bar[0]:
    foo = 42
    bar[0] = false
assert a == 0
assert foo[0] == true
"""
	restoration = Block()
	for arg in preserving.Arguments:
		temp = ReferenceExpression(Context.GetUniqueName("preserving"))
		yield [| $temp = $arg |]
		restoration.Add([| $arg = $temp |])
		
	yield [|
		try:
			$(preserving.Body)
		ensure:
			$restoration
	|]
