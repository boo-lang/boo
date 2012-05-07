using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests.Ast
{
	[TestFixture]
	public class GenericParameterDeclarationTest
	{
		[Test]
		public void LiftReferenceExpression()
		{
			var referenceExpression = new ReferenceExpression("foo");
			var parameter = GenericParameterDeclaration.Lift(referenceExpression);
			Assert.AreEqual(referenceExpression.Name, parameter.Name);
		}

		[Test]
		public void LiftSimpleTypeReference()
		{
			var simpleTypeRef = new SimpleTypeReference("foo");
			var parameter = GenericParameterDeclaration.Lift(simpleTypeRef);
			Assert.AreEqual(simpleTypeRef.Name, parameter.Name);
		}
	}
}