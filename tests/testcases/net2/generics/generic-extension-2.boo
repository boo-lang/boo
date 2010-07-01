"""
foo(bar)
"""
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

[Extension]
def WithArgument[of T(MethodInvocationExpression)](node as T, arg as Expression):
	node.Arguments.Add(arg)
	return node
	
code = [| foo() |].WithArgument([| bar |])
print code.ToCodeString()
