#region license
// Copyright (c) 2003, 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion


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
		stmt.ProtectedBlock = macro.Body
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
		
		

