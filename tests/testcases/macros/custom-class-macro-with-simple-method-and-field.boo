"""
test
"""
import Boo.Lang.Compiler.Ast

macro custom_class(name as ReferenceExpression):
	yield [|
		class $name:
			$(custom_class.Body)
	|]
	
custom_class Foo:
	_msg as string
	def constructor(msg):
		_msg = msg
	def ToString():
		return _msg
		
f = Foo("test")
print f
	
