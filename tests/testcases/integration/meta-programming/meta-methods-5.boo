"""
compile time
runtime
"""
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.MetaProgramming

[meta]
def dict(items as (ExpressionPair)):
	print "compile time"
	h = [| {} |]
	for item in items:
		h.Items.Add(
			[| $((item.First as ReferenceExpression).Name): $(item.Second) |])
	return h

typeDef = [|
	class Test:
		def Run():
			print "runtime"
			return dict(A: "foo", B: "bar")
|]

type = compile(typeDef, System.Reflection.Assembly.GetExecutingAssembly())

h = (type() as duck).Run()
assert h == { "A": "foo", "B": "bar" }
