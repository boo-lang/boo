namespace BooCompiler.Tests.Ast

import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class GenericParameterDeclarationTest:
	[Test]
	def LiftReferenceExpression():
		referenceExpression = ReferenceExpression("foo")
		parameter = GenericParameterDeclaration.Lift(referenceExpression)
		Assert.AreEqual(referenceExpression.Name, parameter.Name)

	[Test]
	def LiftSimpleTypeReference():
		simpleTypeRef = SimpleTypeReference("foo")
		parameter = GenericParameterDeclaration.Lift(simpleTypeRef)
		Assert.AreEqual(simpleTypeRef.Name, parameter.Name)
