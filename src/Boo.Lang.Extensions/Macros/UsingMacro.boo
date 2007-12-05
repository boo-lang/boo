#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace Boo.Lang.Extensions

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

public class UsingMacro(AbstractAstMacro):

	private static final DisposableLocalName = '__disposable__'

	
	override def Expand(macro as MacroStatement):
		// try:
		//		assignment
		// 		<the rest>
		// ensure:
		//		...			
		stmt = TryStatement(macro.LexicalInfo)
		stmt.EnsureBlock = Block(macro.LexicalInfo)
		
		for expression as Expression in macro.Arguments:
			reference as Expression
			
			if expression isa ReferenceExpression:
				reference = expression
			elif IsAssignmentToReference(expression):
				reference = cast(BinaryExpression, expression).Left.CloneNode()
				stmt.ProtectedBlock.Add(expression)
			else:
				tempName as string = string.Format('__using{0}__', _context.AllocIndex())
				reference = ReferenceExpression(expression.LexicalInfo, tempName)
				stmt.ProtectedBlock.Add(BinaryExpression(expression.LexicalInfo, BinaryOperatorType.Assign, reference.CloneNode(), expression))
			
			AugmentEnsureBlock(stmt.EnsureBlock, reference)
		
		stmt.ProtectedBlock.Add(macro.Block)
		return stmt

	
	private def AugmentEnsureBlock(block as Block, reference as Expression):
		// if __disposable = <reference> as System.IDisposable:
		stmt = IfStatement()
		stmt.Condition = BinaryExpression(BinaryOperatorType.Assign, ReferenceExpression(DisposableLocalName), TryCastExpression(reference, SimpleTypeReference('System.IDisposable')))
		
		stmt.TrueBlock = Block()
		
		// __disposable.Dispose()
		stmt.TrueBlock.Add(MethodInvocationExpression(MemberReferenceExpression(ReferenceExpression(DisposableLocalName), 'Dispose')))
		
		// __disposable = null
		stmt.TrueBlock.Add(BinaryExpression(BinaryOperatorType.Assign, ReferenceExpression(DisposableLocalName), NullLiteralExpression()))
		
		block.Add(stmt)

	
	private def IsAssignmentToReference(expression as Expression):
		if NodeType.BinaryExpression != expression.NodeType:
			return false
		
		binaryExpression = cast(BinaryExpression, expression)
		return ((BinaryOperatorType.Assign == binaryExpression.Operator) and (binaryExpression.Left isa ReferenceExpression))

