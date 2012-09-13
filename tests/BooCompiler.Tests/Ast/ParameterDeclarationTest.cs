using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class ParameterDeclarationTest
	{
		[Test]
		public void WhenEmptyToStringReturnsEmpty()
		{
			Assert.AreEqual(string.Empty, new ParameterDeclaration().ToString());
		}

		[Test]
		public void LiftReferenceExpression()
		{
			var referenceExpression = new ReferenceExpression("foo");
			var parameter = ParameterDeclaration.Lift(referenceExpression);
			Assert.AreEqual(referenceExpression.Name, parameter.Name);
			Assert.IsNull(parameter.Type);
		}

		[Test]
		public void LiftCastExpression()
		{
			var referenceExpression = new ReferenceExpression("foo");
			var typeReference = new SimpleTypeReference("T");
			Expression cast = new TryCastExpression(referenceExpression, typeReference);
			var parameter = ParameterDeclaration.Lift(cast);
			Assert.AreEqual(referenceExpression.Name, parameter.Name);
			Assert.IsTrue(typeReference.Matches(parameter.Type));
			Assert.AreNotSame(typeReference, parameter.Type);
		}

		[Test]
		public void LiftCastExpressionWithSelfTarget()
		{
			var self = new SelfLiteralExpression();
			var typeReference = new SimpleTypeReference("T");
			Expression cast = new TryCastExpression(self, typeReference);
			var parameter = ParameterDeclaration.Lift(cast);
			Assert.AreEqual("self", parameter.Name);
			Assert.IsTrue(typeReference.Matches(parameter.Type));
			Assert.AreNotSame(typeReference, parameter.Type);
		}
	}
}
