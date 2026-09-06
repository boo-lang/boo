namespace Boo.Lang.Lsp.Tests.Workspace

import System
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class TextDocumentTestFixture:

	private def Document(text as string):
		return TextDocument("file:///t.boo", "boo", 1, text)

	private def AssertAt(document as TextDocument, offset as int, line as int, character as int):
		position = document.PositionAt(offset)
		assert position.Line == line
		assert position.Character == character
		assert document.OffsetAt(position) == offset

	[Test]
	def CountsTheLinesOfAnEmptyDocument():
		assert Document("").LineCount == 1

	[Test]
	def CountsLinesSeparatedByNewlines():
		assert Document("a\nb\nc").LineCount == 3

	[Test]
	def CountsTheEmptyLineAfterATrailingNewline():
		assert Document("a\n").LineCount == 2

	[Test]
	def MapsOffsetsToPositions():
		document = Document("ab\ncd")
		AssertAt(document, 0, 0, 0)
		AssertAt(document, 2, 0, 2)
		AssertAt(document, 3, 1, 0)
		AssertAt(document, 5, 1, 2)

	[Test]
	def MapsPositionsAcrossCarriageReturns():
		document = Document("ab\r\ncd")
		assert document.LineCount == 2
		assert document.OffsetAt(Position(1, 0)) == 4
		assert document.PositionAt(4).Line == 1

	[Test]
	def CountsCharactersInUtf16CodeUnits():
		# The emoji is one code point but two UTF-16 code units, and LSP counts
		# the units.
		document = Document("a" + char.ConvertFromUtf32(0x1F600) + "b")
		assert document.OffsetAt(Position(0, 3)) == 3
		assert document.PositionAt(3).Character == 3
		assert document.LineText(0).Length == 4

	[Test]
	def ClampsACharacterPastTheEndOfTheLine():
		document = Document("ab\ncd")
		assert document.OffsetAt(Position(0, 99)) == 2

	[Test]
	def ClampsALinePastTheEndOfTheDocument():
		document = Document("ab\ncd")
		assert document.OffsetAt(Position(99, 0)) == 5

	[Test]
	def ClampsANegativePosition():
		assert Document("ab").OffsetAt(Position(-1, -1)) == 0

	[Test]
	def ClampsAnOffsetPastTheEnd():
		document = Document("ab")
		assert document.PositionAt(99).Character == 2

	[Test]
	def ReturnsLineTextWithoutItsTerminator():
		document = Document("ab\r\ncd\n")
		assert document.LineText(0) == "ab"
		assert document.LineText(1) == "cd"
		assert document.LineText(2) == ""

	[Test]
	def ReturnsEmptyTextForALineThatIsNotThere():
		assert Document("ab").LineText(9) == ""

	[Test]
	def ReindexesWhenTheTextChanges():
		document = Document("ab")
		document.Update(2, "ab\ncd\nef")
		assert document.Version == 2
		assert document.LineCount == 3
		assert document.OffsetAt(Position(2, 1)) == 7
