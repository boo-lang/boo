"""
def foo(m as (string)):
	print(m)
"""
import Boo.Lang.Compiler.Ast
typeRef = ArrayTypeReference(SimpleTypeReference("string"))
print [|
	def foo(m as $typeRef):
		print(m)
|].ToCodeString()
