namespace Statsia.Lang.Compiler.Steps

import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.TypeSystem
import System.Linq.Enumerable

class ProcessMethodBodies(Boo.Lang.Compiler.Steps.ProcessMethodBodies):
	# This is the method that the compiler pipeline will execute
	override def Run():
		# The Visitor pattern makes it easy to pick and choose # what we want to process
		super.Visit(CompileUnit)
		
	def HasArray(arguments as ExpressionCollection):
		return arguments.Any({x | GetExpressionType(x).IsArray })
				
	override def ProcessMethodInvocationWithInvalidParameters(node as MethodInvocationExpression, targetMethod as IMethod) as bool:
		// Somehow the generics are not getting picked up directly on the NodeCollection.
		typedArguments = node.Arguments.ToArray().Select({ x | GetExpressionType(x)})		
		if node.Arguments.Count == 1:
			code = [| $(node.Arguments.First).Select({x | return $(node.Target)(x)}).ToArray() |]
			Visit(code)
			node.ParentNode.Replace(node, code)
			return true
		return false
