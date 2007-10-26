"""
(a as Foo)
(a as IFoo)
(a as SFoo)
(a as Foo[of T])
(a as IFoo[of T])
(a as SFoo[of T])
"""
import Boo.Lang.Compiler.Ast

def test(e as TypeDefinition):
	code = [| a as $e |]
	print code.ToCodeString()

typeDef1 = [|
	class Foo:
		pass
|]

typeDef2 = [|
	interface IFoo:
		pass
|]

typeDef3 = [|
	struct SFoo:
		pass
|]

typeDef4 = [|
	class Foo of T:
		pass
|]

typeDef5 = [|
	class IFoo of T:
		pass
|]

typeDef6 = [|
	struct SFoo of T:
		pass
|]

for typeDef in typeDef1, typeDef2, typeDef3, typeDef4, typeDef5, typeDef6:
	test typeDef


