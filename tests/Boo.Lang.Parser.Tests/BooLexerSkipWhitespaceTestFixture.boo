namespace Boo.Lang.Parser.Tests

import System.Collections.Generic
import Antlr4.Runtime
import NUnit.Framework
import Boo.Lang.Parser

[TestFixture]
public class BooLexerSkipWhitespaceTestFixture:
	"""
	A bracket opens a region where the lexer hides whitespace, which is what
	lets an expression span lines. The region is a count, and these fix what
	happens to it when the brackets do not match.
	"""
	private static def NewlinesHidden(source as string) as List[of bool]:
		"""Whether each NEWLINE in the source was hidden, in order."""
		lexer = BooLexer(AntlrInputStream(source))
		lexer.TokenFactory = BooToken.CreateTokenFactory(ParserSettings.DefaultTabSize)

		hidden = List[of bool]()
		token = lexer.NextToken()
		while token.Type != TokenConstants.EOF:
			if token.Type == BooLexer.NEWLINE:
				hidden.Add(token.Channel == TokenConstants.HiddenChannel)
			token = lexer.NextToken()
		return hidden

	[Test]
	public def NewlineInsideParensIsHidden():
		Assert.AreEqual((true, false), NewlinesHidden("b = (2 +\n\t3)\n"))

	[Test]
	public def NewlineOutsideParensEndsTheLine():
		Assert.AreEqual((false, false), NewlinesHidden("a = 1\nb = 2\n"))

	[Test]
	public def NestedParensNeedBothClosed():
		Assert.AreEqual((true, true, false), NewlinesHidden("b = ((2 +\n\t3) +\n\t4)\n"))

	[Test]
	public def AnUnmatchedCloseDoesNotDisableLineContinuation():
		// The count must not go below zero: if it does, the open paren on
		// the second line only brings it back to zero, the newline after it
		// stays visible, and the expression breaks across lines.
		Assert.AreEqual((false, true, false), NewlinesHidden("a = 1)\nb = (2 +\n\t3)\n"))

	[Test]
	public def SeveralUnmatchedClosesDoNotAccumulate():
		Assert.AreEqual((false, true, false), NewlinesHidden("a = 1)))\nb = (2 +\n\t3)\n"))
