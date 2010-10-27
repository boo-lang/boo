"""
First
Second
First Second
"""
import Boo.Lang.Compiler.Ast

macro custom_class(name as ReferenceExpression):
	yield [|
		class $name:
			$(custom_class.Body)
	|]
	
custom_class Foo:
	property First = Init("First")
	property Second = Init("Second")
	
def Init(value):
	print value
	return value
		
f = Foo()
print f.First, f.Second
	
