#region license
// Copyright (c) 2009 Rodrigo B. de Oliveira (rbo@acm.org)
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
		if condition.Operator in (BinaryOperatorType.Equality, BinaryOperatorType.Inequality):
			lft = condition.Left.ToString()

			if not Parameters.Defines.ContainsKey(lft):
				return false

			rgt as string
			if condition.Right isa ReferenceExpression:
				rgt = (condition.Right as ReferenceExpression).Name
			elif condition.Right isa StringLiteralExpression:
				rgt = (condition.Right as StringLiteralExpression).Value
			else:
				rgt = condition.Right.ToString()
				
			if condition.Operator == BinaryOperatorType.Equality:
				return Parameters.Defines[lft] == rgt
			else:
				return Parameters.Defines[lft] != rgt

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