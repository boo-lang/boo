using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace Boo.Lang.Parser.Tests
{
	[TestFixture]
	public class PrimitiveParserTest
	{
		[Test]
		public void LongBoundary()
		{
			IntegerLiteralExpression expression = PrimitiveParser.ParseIntegerLiteralExpression(new BooToken(), "-9223372036854775808L", true);
			Assert.AreEqual(-9223372036854775808L, expression.Value);
		}
	}
}
