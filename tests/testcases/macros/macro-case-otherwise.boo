"""
false
yeah
NO SOUP FOR YOU!
"""
import Boo.Lang.Compiler
import Boo.Lang.PatternMatching

macro _assert:
	case [| _assert $condition |]:
		yield [| raise $(condition.ToCodeString()) if not $condition |]
	case [| _assert $condition, $message |]:
		yield [| raise $message if not $condition |]
	otherwise:
		yield [| print 'NO SOUP FOR YOU!' |]
		
def shield(block as callable()):
	try:
		block()
	except x:
		print x.Message
		
shield:
	_assert false

shield:
	_assert false, "yeah"
	
_assert "invalid", "number", "of", "arguments"
