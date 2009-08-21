"""
false
yeah
BCE0045: Macro expansion error: `_assert` failed to match `_assert 'invalid', 'number', 'of', 'arguments'`.
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.MetaProgramming
import Boo.Lang.PatternMatching

macro _assert:
	case [| _assert $condition |]:
		yield [| raise $(condition.ToCodeString()) if not $condition |]
	case [| _assert $condition, $message |]:
		yield [| raise $message if not $condition |]
		
def shield(block as callable()):
	try:
		block()
	except x:
		print x.Message
		
shield:
	_assert false

shield:
	_assert false, "yeah"
		
code = [|
	namespace test
	_assert true
	_assert "invalid", "number", "of", "arguments"
|]
result = compile_(code, System.Reflection.Assembly.GetExecutingAssembly())
print result.Errors.ToString()

