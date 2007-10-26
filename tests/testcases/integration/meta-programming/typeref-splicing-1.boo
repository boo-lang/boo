"""
(a as Foo)
(a as Foo.Bar)
(a as Foo.Bar)
"""
import Boo.Lang.Compiler.Ast

def test(e as Expression):
	code = [| a as $e |]
	print code.ToCodeString()

test([| Foo |])
test([| Foo.Bar |])
test([| typeof(Foo.Bar) |])
