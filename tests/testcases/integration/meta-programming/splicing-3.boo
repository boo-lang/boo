"""
def foo(a, b):
	return (a + b)
"""
import Boo.Lang.Compiler.Ast

literal = [|
	def foo(a, b):
		// splice(qq(x)) => x
		return $(
			BinaryExpression(
				BinaryOperatorType.Addition,
				[| a |],
				[| b |]))
|]

print literal.ToCodeString()
