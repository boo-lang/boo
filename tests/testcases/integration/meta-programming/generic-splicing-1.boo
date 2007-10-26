"""
Foo[of int]()
(value as Foo[of int])
"""
import Boo.Lang.Compiler.Ast

e = [| Foo of int() |]
assert e isa MethodInvocationExpression
print e.ToCodeString()
c = [| value as $(e.Target) |]
assert c isa TryCastExpression
print c.ToCodeString()
