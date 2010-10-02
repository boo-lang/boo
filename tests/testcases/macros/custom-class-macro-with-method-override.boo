"""
18
Foo.ToString
"""
interface CustomClass:
	LineNumber as int:
		get

macro custom_class(name as Boo.Lang.Compiler.Ast.ReferenceExpression):
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
	
