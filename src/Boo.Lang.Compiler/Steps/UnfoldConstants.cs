#region license
// Copyright (c) 2003, 2004, 2005 Rodrigo B. de Oliveira (rbo@acm.org)
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


using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	/// <summary>
	/// </summary>
	public class UnfoldConstants : AbstractTransformerCompilerStep
	{
		public UnfoldConstants()
		{
		}

		public override void Run()
		{
			if (Errors.Count > 0) return;
			Visit(CompileUnit);
		}

		public override void LeaveBinaryExpression(BinaryExpression node)
		{	
			switch (node.Operator)
			{
				case BinaryOperatorType.BitwiseOr:
					LeaveBitwiseOr(node);
					break;
			}
		}

		private object GetLiteral(Expression expression)
		{	
			switch (expression.NodeType)
			{
				case NodeType.CastExpression:
					return GetLiteral(((CastExpression)expression).Target);
				case NodeType.IntegerLiteralExpression:
					return ((IntegerLiteralExpression)expression).Value;
			}
			IField field = TypeSystemServices.GetOptionalEntity(expression) as IField;
			if (field == null) return null;
			if (!field.IsLiteral) return null;
			return field.StaticValue;
		}

		private void LeaveBitwiseOr(BinaryExpression node)
		{
			IType type = TypeSystemServices.GetExpressionType(node);
			if (!type.IsEnum) return;

			object lhs = GetLiteral(node.Left);
			if (null == lhs) return;

			object rhs = GetLiteral(node.Right);
			if (null == rhs) return;

			ReplaceCurrentNode(
				CodeBuilder.CreateCast(type,
					CodeBuilder.CreateIntegerLiteral(GetLongValue(rhs)|GetLongValue(lhs))));
		}

		private long GetLongValue(object o)
		{
			return (long) Convert.ChangeType(o, typeof(long));
		}

		public override void LeaveUnaryExpression(Boo.Lang.Compiler.Ast.UnaryExpression node)
		{
			switch (node.Operator)
			{
				case UnaryOperatorType.UnaryNegation:
				{
					LeaveUnaryNegation(node);
					break;
				}
			}
		}

		private void LeaveUnaryNegation(UnaryExpression node)
		{
			Expression operand = node.Operand;
			switch (operand.NodeType)
			{
				case NodeType.IntegerLiteralExpression:
					(operand as IntegerLiteralExpression).Value *= -1;
					ReplaceCurrentNode(operand);
					break;
			}
		}
	}
}
