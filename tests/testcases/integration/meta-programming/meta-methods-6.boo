"""
before
test
after
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def code(e as BlockExpression):
	return CodeSerializer().Serialize(e.Body)

module = [|
	import Boo.Lang.Compiler.Ast
	
	[meta] def surround(e as BlockExpression):
		return code:
			print("before")
			// that's how you escape a splice inside a quasi-quotation
			$(SpliceExpression([|e.Body|]))
			print("after")
|]

surroundAssembly = compile(module, System.Reflection.Assembly.GetExecutingAssembly(), typeof(Node).Assembly)

typeDef = [|
	class Test:
		def Run():
			surround:
				print "test"
|]

type = compile(typeDef, surroundAssembly)
(type() as duck).Run()
