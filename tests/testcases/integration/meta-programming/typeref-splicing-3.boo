"""
(a as Foo[of T])
(a as Foo[of T])
"""
import Boo.Lang.Compiler.Ast

def test(e as TypeDefinition):
	code = [| a as $e |]	
	print code.ToCodeString()
	
	oldTypeRef = code.Type
	code = [| a as $(code.Type) |]
	assert code.Type is not oldTypeRef // must be cloned ref
	print code.ToCodeString()

typeDef = [|
	class Foo of T:
		pass
|]
test(typeDef)
