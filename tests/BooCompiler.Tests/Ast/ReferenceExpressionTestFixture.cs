using System;
using NUnit.Framework;
using Boo.Lang.Compiler.Ast;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class ReferenceExpressionTestFixture
	{
		[Test]
		public void LiftSimpleNameString()
		{
			AstAssert.Matches(
				new ReferenceExpression("Foo"),
				ReferenceExpression.Lift("Foo"));
		}

		[Test]
		public void LiftQualifiedNameString()
		{
			AstAssert.Matches(
				new MemberReferenceExpression(
					new ReferenceExpression("Foo"),
					"Bar"),
				ReferenceExpression.Lift("Foo.Bar"));
		}
	}
}
