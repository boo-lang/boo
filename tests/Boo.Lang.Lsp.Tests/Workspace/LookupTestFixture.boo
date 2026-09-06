namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.IO
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

	static final Documented = (
		"class Greeter:\n" +                        # line 0
		'"""A greeter."""\n' +                      # line 1
		"\n" +                                      # line 2
		"\tdef Hello(who as string) as string:\n" + # line 3
		'\t"""Says hello to who."""\n' +            # line 4
		"\t\treturn who\n" +                        # line 5
		"\n" +                                      # line 6
		"g = Greeter()\n" +                         # line 7
		"print g.Hello('x')\n")                     # line 8

	private def Documented_At(line as int, character as int) as Lookup.Result:
		document = TextDocument("file:///doc.boo", "boo", 1, Documented)
		return Lookup.At(document, analyzer.Bound(document), Position(line, character))

	[Test]
	def CarriesTheDocumentationOfAMethod():
		# "print g.Hello('x')", Hello starts at character 8.
		found = Documented_At(8, 8)
		assert found.Documentation == "Says hello to who."

	[Test]
	def CarriesTheDocumentationOfAType():
		# "g = Greeter()", Greeter starts at character 4.
		found = Documented_At(7, 4)
		assert found.Documentation == "A greeter."

	[Test]
	def CarriesNoDocumentationForSomethingUndocumented():
		# who, in "\t\treturn who" at character 9.
		found = Documented_At(5, 9)
		assert found.Name == "who"
		assert found.Documentation is null

	static final Indented = (
		"class Greeter:\n" +           # line 0
		"\n" +                         # line 1
		"\tdef Hello() as string:\n" + # line 2
		'\t"""\n' +                    # line 3
		"\tSays hello.\n" +            # line 4
		"\n" +                         # line 5
		"\tTwice, even.\n" +           # line 6
		'\t"""\n' +                    # line 7
		"\t\treturn 'x'\n" +           # line 8
		"\n" +                         # line 9
		"print Greeter().Hello()\n")   # line 10

	[Test]
	def ReadsDocumentationWithoutTheIndentationItWasWrittenAt():
		document = TextDocument("file:///indented.boo", "boo", 1, Indented)
		found = Lookup.At(document, analyzer.Bound(document), Position(10, 16))
		assert found.Name == "Hello"
		assert found.Documentation == "Says hello.\n\nTwice, even."

	static final Declarations = (
		"import System.IO\n" +                        # line 0
		"\n" +                                        # line 1
		"def Scan(root as string):\n" +               # line 2
		"\tfor name in Directory.GetFiles(root):\n" + # line 3
		"\t\tprint name\n")                           # line 4

	private def Declared_At(line as int, character as int) as Lookup.Result:
		document = TextDocument("file:///declared.boo", "boo", 1, Declarations)
		return Lookup.At(document, analyzer.Bound(document), Position(line, character))

	[Test]
	def DescribesALoopVariableWhereItIsDeclared():
		# "\tfor name in ...", name starts at character 5.
		found = Declared_At(3, 5)
		assert found.Name == "name"
		assert found.Signature == "name as string"

	[Test]
	def DescribesAParameterWhereItIsDeclared():
		# "def Scan(root as string):", root starts at character 9.
		found = Declared_At(2, 9)
		assert found.Name == "root"
		assert found.Signature == "root as string"

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
	def ListsTheOnlyWayAMethodCanBeCalled():
		# "print g.Hello('x')", Hello starts at character 8.
		found = At(8, 8)
		assert found.Overloads.Count == 1
		assert found.Overloads[0].Label == "def Hello(who as string) as string"
		assert found.Overloads[0].Parameters[0] == "who as string"

	[Test]
	def ListsNoWayOfCallingSomethingThatIsNotAMethod():
		found = At(4, 2)
		assert found.Overloads.Count == 0

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
	def PointsSomethingOutsideTheProjectAtDecompiledSource():
		# "print Console.Out", Console starts at character 6.
		found = At(9, 6)
		assert found.HasDeclaration
		assert found.DeclarationUri.EndsWith("System.Console.cs"), found.DeclarationUri
		assert found.Declaration.Line >= 0

	[Test]
	def PointsAnExternalMethodAtTheLineThatDeclaresIt():
		# "for f in Directory.GetFiles('.'):", GetFiles starts at character 19.
		text = "import System.IO\n\nfor f in Directory.GetFiles('.'):\n\tprint f\n"
		document = TextDocument("file:///external.boo", "boo", 1, text)
		found = Lookup.At(document, analyzer.Bound(document), Position(2, 19))
		assert found.Name == "GetFiles"
		assert found.DeclarationUri.EndsWith("System.IO.Directory.cs"), found.DeclarationUri
		lines = File.ReadAllLines(Uri(found.DeclarationUri).LocalPath)
		assert "GetFiles" in lines[found.Declaration.Line], lines[found.Declaration.Line]

	[Test]
	def CoversTheWholeNameWithItsRange():
		found = At(4, 10)
		assert found.Start.Character == 10
		assert found.End.Character == 13

	[Test]
	def PointsAnAttributeAtWhatItNames():
	"""
	An attribute is written as a name like any other, and is the one place
	a type is named without a reference expression to carry it.
	"""
		# "[Obsolete]" on line 1, the name starts at character 1.
		text = "import System\n[Obsolete]\ndef gone():\n\tpass\n"
		document = TextDocument("file:///attributed.boo", "boo", 1, text)
		found = Lookup.At(document, analyzer.Bound(document), Position(1, 2))
		assert found is not null, "nothing found on the attribute"
		assert found.Name == "Obsolete", found.Name
		assert found.HasDeclaration
		assert found.DeclarationUri.EndsWith("System.ObsoleteAttribute.cs"), found.DeclarationUri

	[Test]
	def PointsATypeAnnotationAtWhatItNames():
	"""
	A type written as an annotation is named without a reference
	expression, the same way an attribute is.
	"""
		# "def take(p as Path):", the name starts at character 14.
		text = "import System.IO\n\ndef take(p as Path):\n\tpass\n"
		document = TextDocument("file:///annotated.boo", "boo", 1, text)
		found = Lookup.At(document, analyzer.Bound(document), Position(2, 15))
		assert found is not null, "nothing found on the type"
		assert found.Name == "Path", found.Name
		assert found.HasDeclaration
		assert found.DeclarationUri.EndsWith("System.IO.Path.cs"), found.DeclarationUri
	[Test]
	def FindsTheNameWithTheCursorAtItsEnd():
	"""
	Clicking a name leaves the caret after it, which for a name of one
	character is the only position the editor ever asks about.
	"""
		text = "s = 1\ns.ToString()\n"
		document = TextDocument("file:///edge.boo", "boo", 1, text)
		found = Lookup.At(document, analyzer.Bound(document), Position(1, 1))
		assert found is not null, "nothing found at the end of the name"
		assert found.Name == "s", found.Name
		assert found.HasDeclaration
		assert found.Declaration.Line == 0

	[Test]
	def PointsAtAValueTypeBuiltWithNoArguments():
	"""
	The construction is replaced by an eval over a temp local, which carries
	neither the type's entity nor the column the type was written at.
	"""
		text = "struct Wrapper:\n\tpublic n as int\n\nw = Wrapper()\n"
		document = TextDocument("file:///wrapper.boo", "boo", 1, text)
		# "w = Wrapper()", Wrapper starts at character 4.
		found = Lookup.At(document, analyzer.Bound(document), Position(3, 4))
		assert found is not null, "nothing found"
		assert found.Name == "Wrapper"
		assert found.Signature == "struct Wrapper", found.Signature
		assert found.HasDeclaration
		assert found.Declaration.Line == 0

	[Test]
	def FindsEveryOccurrenceOfTheNameUnderTheCursor():
		# "sum" is declared on line 3 and used on lines 4 and 5.
		text = "def add(a as int):\n\treturn a\n\nsum = add(1)\nprint sum\nprint sum\n"
		document = TextDocument("file:///occ.boo", "boo", 1, text)
		spans = Lookup.Occurrences(document, analyzer.Bound(document), Position(4, 7))
		assert spans.Count == 3, "found ${spans.Count}"
		assert spans[0].Start.Line == 3
		assert spans[1].Start.Line == 4
		assert spans[2].Start.Line == 5
		assert spans[1].End.Character - spans[1].Start.Character == 3

	[Test]
	def LeavesOtherNamesOutOfTheOccurrences():
		text = "sum = 1\ntotal = 2\nprint sum\n"
		document = TextDocument("file:///occ2.boo", "boo", 1, text)
		spans = Lookup.Occurrences(document, analyzer.Bound(document), Position(0, 1))
		assert spans.Count == 2, "found ${spans.Count}"


	[Test]
	def FindsTheNameWithTheCursorAtItsEnd():
	"""
	Clicking a name leaves the caret after it, which for a name of one
	character is the only position the editor ever asks about.
	"""
		text = "s = 1\ns.ToString()\n"
		document = TextDocument("file:///edge.boo", "boo", 1, text)
		found = Lookup.At(document, analyzer.Bound(document), Position(1, 1))
		assert found is not null, "nothing found at the end of the name"
		assert found.Name == "s", found.Name
		assert found.HasDeclaration
		assert found.Declaration.Line == 0

	[Test]
	def TellsWritingAnOccurrenceFromReadingIt():
		text = "s = 1\ns = 2\nprint s\n"
		document = TextDocument("file:///kind.boo", "boo", 1, text)
		spans = Lookup.Occurrences(document, analyzer.Bound(document), Position(2, 7))
		assert spans.Count == 3, "found ${spans.Count}"
		assert spans[0].Written, "line 0 assigns"
		assert spans[1].Written, "line 1 assigns"
		assert not spans[2].Written, "line 2 reads"
