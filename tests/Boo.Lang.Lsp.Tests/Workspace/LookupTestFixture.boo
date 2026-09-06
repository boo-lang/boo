namespace Boo.Lang.Lsp.Tests.Workspace

import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class LookupTestFixture:
"""
Finds what the cursor is on. Hover and go to definition both come from this.
"""

	analyzer as Analyzer

	static final Source = (
		"import System\n" +                         # line 0
		"\n" +                                      # line 1
		"class Greeter:\n" +                        # line 2
		"\tdef Hello(who as string) as string:\n" + # line 3
		"\t\tcount = who.Length\n" +                # line 4
		"\t\treturn who\n" +                        # line 5
		"\n" +                                      # line 6
		"g = Greeter()\n" +                         # line 7
		"print g.Hello('x')\n" +                    # line 8
		"print Console.Out\n")                      # line 9

	[SetUp]
	def Setup():
		analyzer = Analyzer()

	private def Document():
		return TextDocument("file:///a.boo", "boo", 1, Source)

	private def At(line as int, character as int) as Lookup.Result:
		document = Document()
		return Lookup.At(document, analyzer.Bound(document), Position(line, character))

	[Test]
	def FindsNothingOnBlankSpace():
		assert At(6, 0) is null

	[Test]
	def FindsNothingPastTheEndOfALine():
		assert At(7, 40) is null

	[Test]
	def DescribesALocal():
		# "\t\tcount = who.Length", count starts at character 2.
		found = At(4, 2)
		assert found.Name == "count"
		assert found.Signature == "count as int"

	[Test]
	def DescribesAParameter():
		# who, in "\t\tcount = who.Length" at character 10.
		found = At(4, 10)
		assert found.Name == "who"
		assert found.Signature == "who as string"

	[Test]
	def DescribesAPropertyOfAnExternalType():
		# Length, at character 14 of the same line.
		found = At(4, 14)
		assert found.Name == "Length"
		assert found.Signature == "Length as int"

	[Test]
	def DescribesAMethod():
		# "print g.Hello('x')", Hello starts at character 8.
		found = At(8, 8)
		assert found.Name == "Hello"
		assert found.Signature == "def Hello(who as string) as string"

	[Test]
	def DescribesAType():
		# "print Console.Out", Console starts at character 6.
		found = At(9, 6)
		assert found.Name == "Console"
		assert found.Signature == "class System.Console"

	[Test]
	def IgnoresANodeTheCompilerMovedThere():
		# print becomes a Console.WriteLine call carrying the statement's own
		# position, and there is no such name in the text at that spot.
		assert At(8, 0) is null

	[Test]
	def PointsALocalAtItsDeclaration():
		found = At(8, 6)
		assert found.Name == "g"
		assert found.HasDeclaration
		assert found.Declaration.Line == 7

	[Test]
	def PointsAMethodAtItsDeclaration():
		found = At(8, 8)
		assert found.HasDeclaration
		assert found.Declaration.Line == 3

	[Test]
	def HasNoDeclarationForSomethingOutsideTheProject():
		found = At(9, 6)
		assert not found.HasDeclaration

	[Test]
	def CoversTheWholeNameWithItsRange():
		found = At(4, 10)
		assert found.Start.Character == 10
		assert found.End.Character == 13
