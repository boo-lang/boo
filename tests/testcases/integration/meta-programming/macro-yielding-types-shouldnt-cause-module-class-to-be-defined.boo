import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

macro deftypes:
	for type as ReferenceExpression in deftypes.Arguments:
		yield [|
			class $type:
				pass
		|]
		
code = [|
	namespace test
	deftypes Foo, Bar
|]
asm = compile(code, System.Reflection.Assembly.GetExecutingAssembly())
assert 2 == len(asm.GetTypes()), join(asm.GetTypes(), ", ")
