namespace BooCompiler.Tests.Steps

import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps
import NUnit.Framework

[TestFixture]
class InjectImplicitBooleanConversionsTest:
	[Test]
	def NestedAndIsTreatedAsLogicalCondition():
		left = And(null, null)
		right = And(null, null)
		condition = And(left, right)
		IfStatement(Condition: condition)
		for e in (condition, left, right):
			Assert.IsTrue(InjectImplicitBooleanConversions.IsLogicalCondition(e))

	[Test]
	def NestedOrIsTreatedAsLogicalCondition():
		left = Or(null, null)
		right = Or(null, null)
		condition = And(left, right)
		IfStatement(Condition: condition)
		for e in (left, right):
			Assert.IsTrue(InjectImplicitBooleanConversions.IsLogicalCondition(e))

	[Test]
	def NestedLogicalNotIsTreatedAsLogicalCondition():
		left = Or(null, null)
		right = Or(null, null)
		condition = And(Not(left), Or(right, null))
		IfStatement(Condition: condition)
		for e in (left, right):
			Assert.IsTrue(InjectImplicitBooleanConversions.IsLogicalCondition(e))

	private static def Not(operand as Expression) as Expression:
		return UnaryExpression(Operand: operand, Operator: UnaryOperatorType.LogicalNot)

	private static def And(left as Expression, right as Expression) as BinaryExpression:
		return BinaryExpression(BinaryOperatorType.And, left, right)

	private static def Or(left as Expression, right as Expression) as BinaryExpression:
		return BinaryExpression(BinaryOperatorType.Or, left, right)
