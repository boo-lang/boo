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

using System;
using Boo.Lang.Compiler.TypeSystem;

namespace Boo.Lang.Compiler.Steps
{
	using Boo.Lang.Compiler.Ast;

	//TODO: constant propagation (which allows precise unreachable branch detection)
	public class ConstantFolding : AbstractTransformerCompilerStep
	{
		public const string FoldedExpression = "foldedExpression";

		override public void Run()
		{
			if (Errors.Count > 0)
				return;
			Visit(CompileUnit);
		}

		override public void OnModule(Module node)
		{
			Visit(node.Members);
		}

		object GetLiteralValue(Expression node)
		{
			switch (node.NodeType)
			{
				case NodeType.CastExpression:
					return GetLiteralValue(((CastExpression) node).Target);

				case NodeType.BoolLiteralExpression:
					return ((BoolLiteralExpression) node).Value;

				case NodeType.IntegerLiteralExpression:
					return ((IntegerLiteralExpression) node).Value;

				case NodeType.DoubleLiteralExpression:
					return ((DoubleLiteralExpression) node).Value;

				case NodeType.MemberReferenceExpression:
				{
					IField field = TypeSystemServices.GetOptionalEntity(node) as IField;
					if (null != field && field.IsLiteral)
					{
						if (field.Type.IsEnum)
						{
							object o = field.StaticValue;
							if (null != o && o != Error.Default)
								return o;
						}
						else
						{
							Expression e = field.StaticValue as Expression;
							return (null != e)
							       ? GetLiteralValue(e)
							       : field.StaticValue;
						}
					}
					break;
				}
			}
			return null;
		}

		override public void LeaveEnumMember(EnumMember node)
		{
			if (node.Initializer.NodeType == NodeType.IntegerLiteralExpression)
				return;

			IType type = node.Initializer.ExpressionType;
			if (null != type && (TypeSystemServices.IsIntegerNumber(type) || type.IsEnum))
			{
				object val = GetLiteralValue(node.Initializer);
				if (null != val && val != Error.Default)
					node.Initializer = new IntegerLiteralExpression(Convert.ToInt64(val));
			}
			return;
		}

		override public void LeaveBinaryExpression(BinaryExpression node)
		{
			if (AstUtil.GetBinaryOperatorKind(node.Operator) == BinaryOperatorKind.Assignment
			    || node.Operator == BinaryOperatorType.ReferenceEquality
			    || node.Operator == BinaryOperatorType.ReferenceInequality)
				return;

			if (null == node.Left.ExpressionType || null == node.Right.ExpressionType)
				return;

			object lhs = GetLiteralValue(node.Left);
			object rhs = GetLiteralValue(node.Right);
			if (null == lhs || null == rhs)
				return;

			Expression folded = null;
			IType lhsType = GetExpressionType(node.Left);
			IType rhsType = GetExpressionType(node.Right);

			if (TypeSystemServices.BoolType == lhsType && TypeSystemServices.BoolType == rhsType)
			{
				folded = GetFoldedBoolLiteral(node.Operator, Convert.ToBoolean(lhs), Convert.ToBoolean(rhs));
			}
			else if (TypeSystemServices.DoubleType == lhsType || TypeSystemServices.SingleType == lhsType
				|| TypeSystemServices.DoubleType == rhsType || TypeSystemServices.SingleType == rhsType)
			{
				folded = GetFoldedDoubleLiteral(node.Operator, Convert.ToDouble(lhs), Convert.ToDouble(rhs));
			}
			else if (TypeSystemServices.IsIntegerNumber(lhsType) || lhsType.IsEnum)
			{
				bool lhsSigned = TypeSystemServices.IsSignedNumber(lhsType);
				bool rhsSigned = TypeSystemServices.IsSignedNumber(rhsType);
				if (lhsSigned == rhsSigned) //mixed signed/unsigned not supported for folding
				{
					folded = lhsSigned
					         ? GetFoldedIntegerLiteral(node.Operator, Convert.ToInt64(lhs), Convert.ToInt64(rhs))
					         : GetFoldedIntegerLiteral(node.Operator, Convert.ToUInt64(lhs), Convert.ToUInt64(rhs));
				}
			}

			if (null != folded)
			{
				folded.LexicalInfo = node.LexicalInfo;
				folded.ExpressionType = GetExpressionType(node);
				folded.Annotate(FoldedExpression, node);
				ReplaceCurrentNode(folded);
			}
		}

		override public void LeaveUnaryExpression(UnaryExpression node)
		{
			if (node.Operator == UnaryOperatorType.Explode
			    || node.Operator == UnaryOperatorType.AddressOf
			    || node.Operator == UnaryOperatorType.Indirection
			    || node.Operator == UnaryOperatorType.LogicalNot)
				return;

			if (null == node.Operand.ExpressionType)
				return;

			object operand = GetLiteralValue(node.Operand);
			if (null == operand)
				return;

			Expression folded = null;
			IType operandType = GetExpressionType(node.Operand);

			if (TypeSystemServices.DoubleType == operandType || TypeSystemServices.SingleType == operandType)
			{
				folded = GetFoldedDoubleLiteral(node.Operator, Convert.ToDouble(operand));
			}
			else if (TypeSystemServices.IsIntegerNumber(operandType) || operandType.IsEnum)
			{
				folded = GetFoldedIntegerLiteral(node.Operator, Convert.ToInt64(operand));
			}

			if (null != folded)
			{
				folded.LexicalInfo = node.LexicalInfo;
				folded.ExpressionType = GetExpressionType(node);
				ReplaceCurrentNode(folded);
			}
		}

		static BoolLiteralExpression GetFoldedBoolLiteral(BinaryOperatorType @operator, bool lhs, bool rhs)
		{
			bool result;

			switch (@operator)
			{
				//comparison
				case BinaryOperatorType.Equality:
					result = (lhs == rhs);
					break;
				case BinaryOperatorType.Inequality:
					result = (lhs != rhs);
					break;

				//bitwise
				case BinaryOperatorType.BitwiseOr:
					result = lhs | rhs;
					break;
				case BinaryOperatorType.BitwiseAnd:
					result = lhs & rhs;
					break;
				case BinaryOperatorType.ExclusiveOr:
					result = lhs ^ rhs;
					break;

				//logical
				case BinaryOperatorType.And:
					result = lhs && rhs;
					break;
				case BinaryOperatorType.Or:
					result = lhs || rhs;
					break;

				default:
					return null; //not supported
			}
			return new BoolLiteralExpression(result);
		}

		static LiteralExpression GetFoldedIntegerLiteral(BinaryOperatorType @operator, long lhs, long rhs)
		{
			long result;

			switch (@operator)
			{
				//arithmetic
				case BinaryOperatorType.Addition:
					result = lhs + rhs;
					break;
				case BinaryOperatorType.Subtraction:
					result = lhs - rhs;
					break;
				case BinaryOperatorType.Multiply:
					result = lhs * rhs;
					break;
				case BinaryOperatorType.Division:
					result = lhs / rhs;
					break;
				case BinaryOperatorType.Modulus:
					result = lhs % rhs;
					break;
				case BinaryOperatorType.Exponentiation:
					result = (long) Math.Pow(lhs, rhs);
					break;

				//bitwise
				case BinaryOperatorType.BitwiseOr:
					result = lhs | rhs;
					break;
				case BinaryOperatorType.BitwiseAnd:
					result = lhs & rhs;
					break;
				case BinaryOperatorType.ExclusiveOr:
					result = lhs ^ rhs;
					break;
				case BinaryOperatorType.ShiftLeft:
					result = lhs << (int) rhs;
					break;
				case BinaryOperatorType.ShiftRight:
					result = lhs >> (int) rhs;
					break;

				//comparison
				case BinaryOperatorType.LessThan:
					return new BoolLiteralExpression(lhs < rhs);
				case BinaryOperatorType.LessThanOrEqual:
					return new BoolLiteralExpression(lhs <= rhs);
				case BinaryOperatorType.GreaterThan:
					return new BoolLiteralExpression(lhs > rhs);
				case BinaryOperatorType.GreaterThanOrEqual:
					return new BoolLiteralExpression(lhs >= rhs);
				case BinaryOperatorType.Equality:
					return new BoolLiteralExpression(lhs == rhs);
				case BinaryOperatorType.Inequality:
					return new BoolLiteralExpression(lhs != rhs);

				default:
					return null; //not supported
			}
			return new IntegerLiteralExpression(result);
		}

		static LiteralExpression GetFoldedIntegerLiteral(BinaryOperatorType @operator, ulong lhs, ulong rhs)
		{
			ulong result;

			switch (@operator)
			{
				//arithmetic
				case BinaryOperatorType.Addition:
					result = lhs + rhs;
					break;
				case BinaryOperatorType.Subtraction:
					result = lhs - rhs;
					break;
				case BinaryOperatorType.Multiply:
					result = lhs * rhs;
					break;
				case BinaryOperatorType.Division:
					result = lhs / rhs;
					break;
				case BinaryOperatorType.Modulus:
					result = lhs % rhs;
					break;
				case BinaryOperatorType.Exponentiation:
					result = (ulong) Math.Pow(lhs, rhs);
					break;

				//bitwise
				case BinaryOperatorType.BitwiseOr:
					result = lhs | rhs;
					break;
				case BinaryOperatorType.BitwiseAnd:
					result = lhs & rhs;
					break;
				case BinaryOperatorType.ExclusiveOr:
					result = lhs ^ rhs;
					break;
				case BinaryOperatorType.ShiftLeft:
					result = lhs << (int) rhs;
					break;
				case BinaryOperatorType.ShiftRight:
					result = lhs >> (int) rhs;
					break;

				//comparison
				case BinaryOperatorType.LessThan:
					return new BoolLiteralExpression(lhs < rhs);
				case BinaryOperatorType.LessThanOrEqual:
					return new BoolLiteralExpression(lhs <= rhs);
				case BinaryOperatorType.GreaterThan:
					return new BoolLiteralExpression(lhs > rhs);
				case BinaryOperatorType.GreaterThanOrEqual:
					return new BoolLiteralExpression(lhs >= rhs);
				case BinaryOperatorType.Equality:
					return new BoolLiteralExpression(lhs == rhs);
				case BinaryOperatorType.Inequality:
					return new BoolLiteralExpression(lhs != rhs);

				default:
					return null; //not supported
			}
			return new IntegerLiteralExpression((long) result);
		}

		static LiteralExpression GetFoldedDoubleLiteral(BinaryOperatorType @operator, double lhs, double rhs)
		{
			double result;

			switch (@operator)
			{
				//arithmetic
				case BinaryOperatorType.Addition:
					result = lhs + rhs;
					break;
				case BinaryOperatorType.Subtraction:
					result = lhs - rhs;
					break;
				case BinaryOperatorType.Multiply:
					result = lhs * rhs;
					break;
				case BinaryOperatorType.Division:
					result = lhs / rhs;
					break;
				case BinaryOperatorType.Modulus:
					result = lhs % rhs;
					break;
				case BinaryOperatorType.Exponentiation:
					result = Math.Pow(lhs, rhs);
					break;

				//comparison
				case BinaryOperatorType.LessThan:
					return new BoolLiteralExpression(lhs < rhs);
				case BinaryOperatorType.LessThanOrEqual:
					return new BoolLiteralExpression(lhs <= rhs);
				case BinaryOperatorType.GreaterThan:
					return new BoolLiteralExpression(lhs > rhs);
				case BinaryOperatorType.GreaterThanOrEqual:
					return new BoolLiteralExpression(lhs >= rhs);
				case BinaryOperatorType.Equality:
					return new BoolLiteralExpression(lhs == rhs);
				case BinaryOperatorType.Inequality:
					return new BoolLiteralExpression(lhs != rhs);

				default:
					return null; //not supported
			}
			return new DoubleLiteralExpression(result);
		}

		static LiteralExpression GetFoldedIntegerLiteral(UnaryOperatorType @operator, long operand)
		{
			long result;

			switch (@operator)
			{
				case UnaryOperatorType.UnaryNegation:
					result = -operand;
					break;
				case UnaryOperatorType.OnesComplement:
					result = ~operand;
					break;

				default:
					return null; //not supported
			}
			return new IntegerLiteralExpression(result);
		}

		static LiteralExpression GetFoldedDoubleLiteral(UnaryOperatorType @operator, double operand)
		{
			double result;

			switch (@operator)
			{
				case UnaryOperatorType.UnaryNegation:
					result = -operand;
					break;

				default:
					return null; //not supported
			}
			return new DoubleLiteralExpression(result);
		}
	}
}

