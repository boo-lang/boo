namespace Boo.Lang.Parser.Tests

import Boo.Lang.Compiler.Ast
import NUnit.Framework
import Boo.Lang.Parser

[TestFixture]
class PrimitiveParserTest:
	[Test]
	def LongBoundary():
		expression = PrimitiveParser.ParseIntegerLiteralExpression(LexicalInfo.Empty, "-9223372036854775808L", true)
		Assert.AreEqual(-9223372036854775808L, expression.Value)
