"""
Foo.ToString
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching

macro custom_class(name as ReferenceExpression):
	yield [|
		class $name:
			$(custom_class.Body)
	|]
	
custom_class Foo:
	def ToString():
		return "Foo.ToString"
		
f = Foo()
print f
	
