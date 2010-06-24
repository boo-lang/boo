using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace Nih.Tests
{
	[TestFixture]
	public class ParserTest
	{
		[Test]
		public void Say42IsParsedAsMethodInvocationExpression()
		{
			var module = Nih.Parser.ParseModule("say 42");

			Assert.AreEqual(0, module.Members.Count);

			Assert.AreEqual(1, module.Globals.Statements.Count);

			var stmt = (ExpressionStatement)module.Globals.Statements[0];

			var invocation = (MethodInvocationExpression)stmt.Expression;
			Assert.AreEqual("say", ((ReferenceExpression)invocation.Target).Name);
			Assert.AreEqual(1, invocation.Arguments.Count);
			Assert.AreEqual(42, ((IntegerLiteralExpression)invocation.Arguments[0]).Value);
		}
	}
}
