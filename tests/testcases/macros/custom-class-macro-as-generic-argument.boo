"""
[Foo]
"""
import Boo.Lang.Compiler.Ast

macro custom_class(name as ReferenceExpression):
	yield [|
		class $name:
			$(custom_class.Body)
	|]
	
interface I:
	Foos as List of Foo:
		get
	
class Impl(I):
	property Foos = List of Foo()
	
custom_class Foo:
	pass
		
def printFoosOf(i as I):
	print i.Foos

impl = Impl()
impl.Foos.Add(Foo())
printFoosOf impl

