namespace BooCompiler.Tests.Ast

import NUnit.Framework
import Boo.Lang.Compiler.Ast

[TestFixture]
class ReferenceExpressionTestFixture:
	[Test]
	def LiftSimpleNameString():
		AstAssert.Matches(
			ReferenceExpression("Foo"),
			ReferenceExpression.Lift("Foo"))

	[Test]
	def LiftQualifiedNameString():
		AstAssert.Matches(
			MemberReferenceExpression(
				ReferenceExpression("Foo"),
				"Bar"),
			ReferenceExpression.Lift("Foo.Bar"))
