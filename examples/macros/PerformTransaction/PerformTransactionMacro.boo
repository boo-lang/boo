import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

class PerformTransactionMacro(AbstractAstMacro):
"""
performTransaction connection:
	connection.Execute(cmd1)
	connection.Execute(cmd2)

transaction=connection.BeginTransaction()
try:
	// transaction logic
	transaction.Commit()
except:
	transaction.Revert()
	raise
ensure:
	transaction.End()
"""

	override def Expand(macro as MacroStatement):
		
		assert 1 == len(macro.Arguments)
		
		connection = macro.Arguments[0]
		
		block = Block()
		block.Add(
			BinaryExpression(
				BinaryOperatorType.Assign,
				ReferenceExpression("transaction"),
				MethodInvocationExpression(
					MemberReferenceExpression(
						connection,
						"BeginTransaction"))))
					
		stmt = TryStatement()
		stmt.ProtectedBlock = macro.Block
		stmt.ProtectedBlock.Add(
			MethodInvocationExpression(
				MemberReferenceExpression(
					ReferenceExpression("transaction"),
					"Commit")))
					
		handler = ExceptionHandler()
		handler.Block.Add(
			MethodInvocationExpression(
				MemberReferenceExpression(
					ReferenceExpression("transaction"),
					"Revert")))
		handler.Block.Add(
			RaiseStatement())
		stmt.ExceptionHandlers.Add(handler)
		
		stmt.EnsureBlock = Block()
		stmt.EnsureBlock.Add(
			MethodInvocationExpression(
				MemberReferenceExpression(
					ReferenceExpression("transaction"),
					"End")))
		
		block.Add(stmt)			
						
		return block
		
		

