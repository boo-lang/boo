"""
if not (x == y):
	raise AssertionFailedException('x == y')
"""
import Boo.Lang.Compiler.Ast

def assert(condition as Expression):
	return [|
		if not $condition:
			raise AssertionFailedException($(condition.ToCodeString()))
	|]

print assert([| x == y |]).ToCodeString()
