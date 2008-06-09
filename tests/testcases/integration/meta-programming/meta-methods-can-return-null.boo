"""
compile time: true
compile time: false
runtime
before
during
after
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def surround(enabled as BoolLiteralExpression, block as BlockExpression):
	print "compile time:", enabled
	if not enabled.Value: return null
	return [|
		print("before")
		$(block.Body)
		print("after")
	|]

typeDef = [|
	class Test:
		def Run():
			surround true:
				print "during"
			surround false:
				print "shouldn't see this"
|]

type = compile(typeDef, System.Reflection.Assembly.GetExecutingAssembly())
print "runtime"
(type() as duck).Run()

