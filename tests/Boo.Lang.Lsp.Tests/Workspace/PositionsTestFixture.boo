namespace Boo.Lang.Lsp.Tests.Workspace

import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Compiler.Ast
import Boo.Lang.Lsp.Workspace

[TestFixture]
class PositionsTestFixture:
"""
LSP counts lines and characters from zero; the compiler counts them from one.
Every crossing goes through here.
"""

	[Test]
	def ShiftsACompilerLocationDownToLspNumbering():
		position = Positions.FromSourceLocation(SourceLocation(3, 12))
		assert position.Line == 2
		assert position.Character == 11

	[Test]
	def ShiftsAnLspPositionUpToCompilerNumbering():
		location = Positions.ToLexicalInfo("/t.boo", Position(2, 11))
		assert location.Line == 3
		assert location.Column == 12
		assert location.FileName == "/t.boo"

	[Test]
	def RoundTrips():
		position = Position(7, 4)
		assert Positions.FromSourceLocation(Positions.ToLexicalInfo("/t.boo", position)).Line == 7
		assert Positions.FromSourceLocation(Positions.ToLexicalInfo("/t.boo", position)).Character == 4

	[Test]
	def ReadsAColumnBackThroughATab():
		# The lexer advances a tab to the next tab stop, so column and
		# character part company on every indented line.
		# The parser reports nmae at column 16; it starts at character 12.
		document = TextDocument("file:///a.boo", "boo", 1, "\treturn x + nmae\n")
		position = Positions.FromLexicalInfo(document, LexicalInfo("file:///a.boo", 1, 16))
		assert position.Line == 0
		assert position.Character == 12
		assert document.LineText(0).Substring(position.Character, 4) == "nmae"

	[Test]
	def CountsATabAsOneCharacterAndFourColumns():
		document = TextDocument("file:///a.boo", "boo", 1, "\t\tx = 1\n")
		# Two tabs are two characters and eight columns, so x is character 2.
		position = Positions.FromLexicalInfo(document, LexicalInfo("file:///a.boo", 1, 9))
		assert position.Character == 2

	[Test]
	def PutsAColumnBackWhereTheCompilerWouldHaveIt():
		document = TextDocument("file:///a.boo", "boo", 1, "\treturn x + nmae\n")
		location = Positions.ToLexicalInfo(document, Position(0, 12))
		assert location.Line == 1
		assert location.Column == 16

	[Test]
	def RoundTripsThroughTabs():
		document = TextDocument("file:///a.boo", "boo", 1, "\t\tif a and b:\n")
		for character in range(document.LineText(0).Length):
			position = Position(0, character)
			back = Positions.FromLexicalInfo(document, Positions.ToLexicalInfo(document, position))
			assert back.Character == character

	[Test]
	def LeavesALineWithoutTabsAlone():
		document = TextDocument("file:///a.boo", "boo", 1, "print greet(42)\n")
		assert Positions.FromLexicalInfo(document, LexicalInfo("file:///a.boo", 1, 7)).Character == 6

	[Test]
	def ClampsAColumnPastTheEndOfTheLine():
		document = TextDocument("file:///a.boo", "boo", 1, "x = 1\n")
		assert Positions.FromLexicalInfo(document, LexicalInfo("file:///a.boo", 1, 99)).Character == 5

	[Test]
	def ClampsAnUnsetCompilerLocationToTheStart():
		# LexicalInfo.Empty carries line and column of -1.
		position = Positions.FromSourceLocation(SourceLocation(-1, -1))
		assert position.Line == 0
		assert position.Character == 0
