"""
before
_field0
_field1
_field2
_field3
_field4
_field5
after
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching

macro fields:
	for arg in fields.Arguments:
		fieldName = "_field" + arg
		yield [|
			private $fieldName = DebugInitializerOrder($fieldName)
		|]
		
class Initializers:
	_field0 = DebugInitializerOrder("_field0")
	fields 1, 2
	_field3 = DebugInitializerOrder("_field3")
	fields 4
	_field5 = DebugInitializerOrder("_field5")
		
def DebugInitializerOrder(value):
	print value
	return value
		
print "before"
Initializers()
print "after"
