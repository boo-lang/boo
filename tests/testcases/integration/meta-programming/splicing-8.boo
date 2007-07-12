"""
def foo(m as (string)):
	print(m)
"""
import Boo.Lang.Compiler.Ast
typeRef = SimpleTypeReference("string")
print [|
	def foo(m as ($typeRef)):
		print(m)
|].ToCodeString()
