namespace Boo.Lang.Lsp.Tests.Workspace

import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class BracketRepairTestFixture:
"""
Half typed brackets are the normal state of a buffer, and they cost the parser
everything below them. Repair balances the text before it is parsed for an
outline; what the client sees and what diagnostics report is the real text.
"""

	[Test]
	def LeavesBalancedTextAlone():
		text = "def f():\n\tx = g(1)\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def LeavesTextWithoutBracketsAlone():
		text = "x = 1\ny = 2\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def ClosesAnOpenParenAtTheEndOfItsLine():
		assert BracketRepair.Repair("x = f(\ny = 2\n") == "x = f()\ny = 2\n"

	[Test]
	def ClosesAnOpenBracketAndBrace():
		assert BracketRepair.Repair("x = f[\n") == "x = f[]\n"
		assert BracketRepair.Repair("x = {\n") == "x = {}\n"

	[Test]
	def ClosesSeveralOpenersInTheRightOrder():
		assert BracketRepair.Repair("x = f(g[\n") == "x = f(g[])\n"

	[Test]
	def ClosesAnOpenerOnTheLastLineWithoutANewline():
		assert BracketRepair.Repair("x = f(") == "x = f()"

	[Test]
	def DropsAnUnmatchedCloser():
		assert BracketRepair.Repair("x = 1)\ny = 2\n") == "x = 1 \ny = 2\n"

	[Test]
	def KeepsAMatchedPairOnOneLine():
		text = "x = (1 +\n\t2)\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def IgnoresBracketsInASingleQuotedString():
		text = "x = 'a(b'\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def IgnoresBracketsInADoubleQuotedString():
		text = 'x = "a)b"\n'
		assert BracketRepair.Repair(text) == text

	[Test]
	def IgnoresBracketsInATripleQuotedString():
		text = '"""\na ( b )) c\n"""\nx = 1\n'
		assert BracketRepair.Repair(text) == text

	[Test]
	def IgnoresAnEscapedQuoteInsideAString():
		text = "x = 'a\\'b(c'\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def IgnoresBracketsInAHashComment():
		text = "x = 1 # a ( b\ny = 2\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def IgnoresBracketsInASlashComment():
		text = "x = 1 // a ( b\ny = 2\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def IgnoresBracketsInABlockComment():
		text = "x = 1 /* a ( b */\ny = 2\n"
		assert BracketRepair.Repair(text) == text

	[Test]
	def ClosesAnUnterminatedStringSoTheRestIsNotSwallowed():
		# An unterminated quote would otherwise hide every bracket below it.
		assert BracketRepair.Repair("x = 'oops\ny = f(\n") == "x = 'oops'\ny = f()\n"

	[Test]
	def KeepsTheLineCountUnchanged():
		repaired = BracketRepair.Repair("def f():\n\tx = g(\n\ndef h():\n\tpass\n")
		assert repaired.Split((char(10),)).Length == 6
