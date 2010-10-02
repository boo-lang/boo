"""
21
Foo.ToString
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.PatternMatching

interface CustomClass:
	LineNumber as int:
		get

macro custom_class(name as ReferenceExpression):
	yield [|
		class $name(CustomClass):
			$(custom_class.Body)
			
			LineNumber:
				get: return $(custom_class.LexicalInfo.Line)
	|]
	
custom_class Foo:
	override def ToString():
		return "Foo.ToString"
		
f = Foo()
assert f isa CustomClass
print f.LineNumber
print f
	
