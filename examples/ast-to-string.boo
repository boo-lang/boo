import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import System.IO

def ToString(ast as Node):
	writer=StringWriter()
	BooPrinterVisitor(writer).Visit(ast)
	return writer.ToString()
	
mie=MethodInvocationExpression(ReferenceExpression("print"))
mie.Arguments.Add(StringLiteralExpression("Hello!"))

print(ToString(mie))