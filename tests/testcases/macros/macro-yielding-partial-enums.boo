"""
"""
import System
import Boo.Lang.Compiler.Ast

macro expression(name as ReferenceExpression):
	yield [|
		partial enum ExpressionType:
			$name
	|]
	yield [|
		class $(name + "Expression")(Expression):
			ExpressionType:
				get: return ExpressionType.$name
	|]
	
abstract class Expression:
	abstract ExpressionType as ExpressionType:
		get

expression Infix
expression Postfix

assert InfixExpression().ExpressionType == ExpressionType.Infix
assert PostfixExpression().ExpressionType == ExpressionType.Postfix
	

