"""
None = 0
Infix = 1
Postfix = 2
"""
import System
import Boo.Lang.Compiler.Ast

macro expression(name as ReferenceExpression):
	yield [|
		partial enum ExpressionType:
			$name
	|]
	
partial enum ExpressionType:
	None
	
expression Infix
expression Postfix

for value in Enum.GetValues(ExpressionType):
	print value, "=", value cast int
