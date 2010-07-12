namespace Boo.Lang.Extensions

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class IfdefMacro(AbstractAstMacro):
	
	override def Expand(node as MacroStatement):
		assert len(node.Arguments) == 1, "ifdef <expression>: <statement>+"
		if Evaluate(node.Arguments[0]):
			return node.Body
		return null

	private def Evaluate(condition as Expression) as bool:
		reference = condition as ReferenceExpression
		if reference is not null:
			return EvaluateReference(reference)
			
		unary = condition as UnaryExpression
		if unary is not null:
			return EvaluateUnary(unary)
			
		binary = condition as BinaryExpression
		if binary is not null:
			return EvaluateBinary(binary)
			
		return UnsupportedExpression(condition)
		
	private def EvaluateReference(condition as ReferenceExpression):
		return Parameters.Defines.ContainsKey(condition.Name)

	private def EvaluateBinary(condition as BinaryExpression):
		if condition.Operator == BinaryOperatorType.Or:
			return Evaluate(condition.Left) or Evaluate(condition.Right)
		if condition.Operator == BinaryOperatorType.And:
			return Evaluate(condition.Left) and Evaluate(condition.Right)
		return UnsupportedExpression(condition)

	private def EvaluateUnary(condition as UnaryExpression):
		if condition.Operator == UnaryOperatorType.LogicalNot:
			return not Evaluate(condition.Operand)
		return UnsupportedExpression(condition)

	private def UnsupportedExpression(condition as Expression) as bool:
		raise CompilerError(condition, "Unsupported expression: " + condition.ToCodeString())