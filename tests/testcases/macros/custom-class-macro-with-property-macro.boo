"""
42
"""
import Boo.Lang.Compiler.Ast

macro custom_class(name as ReferenceExpression):
	yield [|
		class $name:
			$(custom_class.Body)
	|]
	
custom_class Foo:
	property Value as int
		
f = Foo(Value: 42)
print f.Value
	
