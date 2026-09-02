namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class ParameterDeclarationTest:
	[Test]
	def WhenEmptyToStringReturnsEmpty():
		Assert.AreEqual(string.Empty, ParameterDeclaration().ToString())

	[Test]
	def LiftReferenceExpression():
		referenceExpression = ReferenceExpression("foo")
		parameter = ParameterDeclaration.Lift(referenceExpression)
		Assert.AreEqual(referenceExpression.Name, parameter.Name)
		Assert.IsNull(parameter.Type)

	[Test]
	def LiftCastExpression():
		referenceExpression = ReferenceExpression("foo")
		typeReference = SimpleTypeReference("T")
		castExpression as Expression = TryCastExpression(referenceExpression, typeReference)
		parameter = ParameterDeclaration.Lift(castExpression)
		Assert.AreEqual(referenceExpression.Name, parameter.Name)
		Assert.IsTrue(typeReference.Matches(parameter.Type))
		Assert.AreNotSame(typeReference, parameter.Type)

	[Test]
	def LiftCastExpressionWithSelfTarget():
		selfLiteral = SelfLiteralExpression()
		typeReference = SimpleTypeReference("T")
		castExpression as Expression = TryCastExpression(selfLiteral, typeReference)
		parameter = ParameterDeclaration.Lift(castExpression)
		Assert.AreEqual("self", parameter.Name)
		Assert.IsTrue(typeReference.Matches(parameter.Type))
		Assert.AreNotSame(typeReference, parameter.Type)
