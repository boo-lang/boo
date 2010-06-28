using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

// ReSharper disable CheckNamespace
namespace Boo.Lang.ConditionalCompilation
// ReSharper restore CheckNamespace
{
	public class IfdefMacro : AbstractAstMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			if (macro.Arguments.Count != 1)
				throw new CompilerError(macro, "ifdef <expression>: <statement>+");

			return Evaluate(macro.Arguments[0]) ? macro.Body : null;
		}

		private bool Evaluate(Expression condition)
		{
			switch (condition.NodeType)
			{
				case NodeType.ReferenceExpression:
					return EvaluateReference((ReferenceExpression) condition);
				case NodeType.UnaryExpression:
					return EvaluateUnary((UnaryExpression) condition);
				case NodeType.BinaryExpression:
					return EvaluateBinary((BinaryExpression) condition);
				default:
					return UnsupportedExpression(condition);
			}
		}

		private bool EvaluateBinary(BinaryExpression condition)
		{
			switch (condition.Operator)
			{
				case BinaryOperatorType.Or:
					return Evaluate(condition.Left) || Evaluate(condition.Right);
				case BinaryOperatorType.And:
					return Evaluate(condition.Left) && Evaluate(condition.Right);
				default:
					return UnsupportedExpression(condition);
			}
		}

		private bool EvaluateUnary(UnaryExpression condition)
		{
			switch (condition.Operator)
			{
				case UnaryOperatorType.LogicalNot:
					return !Evaluate(condition.Operand);
				default:
					return UnsupportedExpression(condition);
			}
		}

		private bool UnsupportedExpression(Expression condition)
		{
			throw new CompilerError(condition, "Unsupported expression: " + condition.ToCodeString());
		}

		private bool EvaluateReference(ReferenceExpression condition)
		{
			return Parameters.Defines.ContainsKey(condition.Name);
		}
	}
}
