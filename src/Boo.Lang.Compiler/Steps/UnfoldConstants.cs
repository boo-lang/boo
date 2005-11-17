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
