using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using NUnit.Framework;

namespace BooCompiler.Tests.Steps
{
	[TestFixture]
	public class InjectImplicitBooleanConversionsTest
	{
		[Test]
		public void NestedAndIsTreatedAsLogicalCondition()
		{
			var left = And(null, null);
			var right = And(null, null);
			var condition = And(left, right);
			new IfStatement { Condition = condition };
			foreach (var e in new[] { condition, left, right })
				Assert.IsTrue(InjectImplicitBooleanConversions.IsLogicalCondition(e));
		}

		[Test]
		public void NestedOrIsTreatedAsLogicalCondition()
		{
			var left = Or(null, null);
			var right = Or(null, null);
			var condition = And(left, right);
			new IfStatement { Condition = condition };
			foreach (var e in new[] { left, right })
				Assert.IsTrue(InjectImplicitBooleanConversions.IsLogicalCondition(e));
		}

		[Test]
		public void NestedLogicalNotIsTreatedAsLogicalCondition()
		{
			var left = Or(null, null);
			var right = Or(null, null);
			var condition = And(Not(left), Or(right, null));
			new IfStatement { Condition = condition };
			foreach (var e in new[] { left, right })
				Assert.IsTrue(InjectImplicitBooleanConversions.IsLogicalCondition(e));
		}

		private static Expression Not(Expression operand)
		{
			return new UnaryExpression { Operand = operand, Operator = UnaryOperatorType.LogicalNot };
		}

		private static BinaryExpression And(Expression left, Expression right)
		{
			return new BinaryExpression(BinaryOperatorType.And, left, right);
		}

		private static BinaryExpression Or(Expression left, Expression right)
		{
			return new BinaryExpression(BinaryOperatorType.Or, left, right);
		}
	}
}
