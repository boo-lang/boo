import System
import Boo.Lang.Ast
import Boo.Lang.Ast.Visitors

def printNode(node as Node):
	BooPrinterVisitor(Console.Out).Switch(node)

be = BinaryExpression(BinaryOperatorType.Assign,
						ReferenceExpression("foo"),
						ReferenceExpression("bar"))
						
clone as BinaryExpression = be.Clone()

print(be.Left.ParentNode is be)
print(be.Right.ParentNode is be)
print(clone.Left.ParentNode is clone)
print(clone.Right.ParentNode is clone)

printNode(be)
printNode(clone)


