using System.Collections.Generic;
using Antlr4.Runtime;
using NUnit.Framework;

namespace Boo.Lang.Parser.Tests
{
	/// <summary>
	/// A bracket opens a region where the lexer hides whitespace, which is what
	/// lets an expression span lines. The region is a count, and these fix what
	/// happens to it when the brackets do not match.
	/// </summary>
	[TestFixture]
	public class BooLexerSkipWhitespaceTestFixture
	{
		/// Whether each NEWLINE in the source was hidden, in order.
		private static List<bool> NewlinesHidden(string source)
		{
			var lexer = new BooLexer(new AntlrInputStream(source))
			{
				TokenFactory = BooToken.CreateTokenFactory(ParserSettings.DefaultTabSize)
			};

			var hidden = new List<bool>();
			for (var token = lexer.NextToken(); token.Type != TokenConstants.EOF; token = lexer.NextToken())
				if (token.Type == BooLexer.NEWLINE)
					hidden.Add(token.Channel == TokenConstants.HiddenChannel);
			return hidden;
		}

		[Test]
		public void NewlineInsideParensIsHidden()
		{
			Assert.AreEqual(new[] { true, false }, NewlinesHidden("b = (2 +\n\t3)\n"));
		}

		[Test]
		public void NewlineOutsideParensEndsTheLine()
		{
			Assert.AreEqual(new[] { false, false }, NewlinesHidden("a = 1\nb = 2\n"));
		}

		[Test]
		public void NestedParensNeedBothClosed()
		{
			Assert.AreEqual(new[] { true, true, false }, NewlinesHidden("b = ((2 +\n\t3) +\n\t4)\n"));
		}

		[Test]
		public void AnUnmatchedCloseDoesNotDisableLineContinuation()
		{
			// The count must not go below zero: if it does, the open paren on
			// the second line only brings it back to zero, the newline after it
			// stays visible, and the expression breaks across lines.
			Assert.AreEqual(new[] { false, true, false }, NewlinesHidden("a = 1)\nb = (2 +\n\t3)\n"));
		}

		[Test]
		public void SeveralUnmatchedClosesDoNotAccumulate()
		{
			Assert.AreEqual(new[] { false, true, false }, NewlinesHidden("a = 1)))\nb = (2 +\n\t3)\n"));
		}
	}
}
