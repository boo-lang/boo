import Boo.Lang.Ast
import Boo.Lang.Ast.Visitors
import System

def print(node as Node):
	BooPrinterVisitor(Console.Out).Switch(node)
	
def CreateNotExpression(e as Expression):
	return UnaryExpression(Operand: e, Operator: UnaryOperatorType.Not)

e = ExpressionStatement(
			Expression: be = BinaryExpression(BinaryOperatorType.Assign,
											ReferenceExpression("a"),
											IntegerLiteralExpression(3)
											)
					)
print(e)

be.ReplaceBy(MethodInvocationExpression(Target: ReferenceExpression("a")))
print(e)


i = IfStatement(Expression: be = BinaryExpression(BinaryOperatorType.NotMatch,
										StringLiteralExpression("foo"),
										StringLiteralExpression("bar")))
i.TrueBlock = Block()
//be.ReplaceBy(CreateNotExpression(be))
//i.Expression = CreateNotExpression(be)
i.Replace(be, CreateNotExpression(be))

be.Operator = BinaryOperatorType.Match
print(i)

