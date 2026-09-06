namespace Boo.Lang.Lsp.Tests.Workspace

import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Workspace

[TestFixture]
class SymbolsTestFixture:

	analyzer as Analyzer

	[SetUp]
	def Setup():
		analyzer = Analyzer()

	private def SymbolsOf(text as string):
		document = TextDocument("file:///a.boo", "boo", 1, text)
		return Symbols.Of(document, analyzer.ParseTree(document))

	private def Named(symbols as List[of object], name as string) as Dictionary[of string, object]:
		for symbol as Dictionary[of string, object] in symbols:
			return symbol if symbol["name"] == name
		return null

	private def Children(symbol as Dictionary[of string, object]):
		return symbol["children"] as List[of object]

	[Test]
	def ReportsNothingForAnEmptyDocument():
		assert SymbolsOf("").Count == 0

	[Test]
	def ReportsAClass():
		greeter = Named(SymbolsOf("class Greeter:\n\tpass\n"), "Greeter")
		assert greeter is not null
		assert greeter["kind"] == Symbols.Class

	[Test]
	def NestsMembersUnderTheirType():
		symbols = SymbolsOf("class Greeter:\n\tdef Hello():\n\t\tpass\n")
		hello = Named(Children(Named(symbols, "Greeter")), "Hello")
		assert hello is not null
		assert hello["kind"] == Symbols.Method

	[Test]
	def ReportsAModuleLevelDefAsAFunction():
		top = Named(SymbolsOf("def Hello():\n\tpass\n"), "Hello")
		assert top["kind"] == Symbols.Function

	[Test]
	def ReportsADefThatFollowsModuleLevelCode():
		# Such a def parses as a declaration in the module's globals rather
		# than as a member, and is easy to lose.
		symbols = SymbolsOf("def Before():\n\tpass\n\nx = 1\n\ndef After():\n\tpass\n")
		assert Named(symbols, "Before") is not null
		assert Named(symbols, "After") is not null
		assert Named(symbols, "After")["kind"] == Symbols.Function

	[Test]
	def SelectsTheNameOfADefBelowModuleLevelCode():
		# Such a def is pointed at its keyword, not its name.
		symbols = SymbolsOf("x = 1\n\ndef After():\n\tpass\n")
		selection = Named(symbols, "After")["selectionRange"] as Dictionary[of string, object]
		start = selection["start"] as Dictionary[of string, object]
		finish = selection["end"] as Dictionary[of string, object]
		assert start["character"] == 4L
		assert finish["character"] == 9L

	[Test]
	def ReportsFieldsAndProperties():
		symbols = SymbolsOf("class Greeter:\n\tname as string\n\n\tGreeting:\n\t\tget: return 'hi'\n")
		members = Children(Named(symbols, "Greeter"))
		assert Named(members, "name")["kind"] == Symbols.Field
		assert Named(members, "Greeting")["kind"] == Symbols.Property

	[Test]
	def ReportsAnEnumAndItsMembers():
		symbols = SymbolsOf("enum Colour:\n\tRed\n\tGreen\n")
		colour = Named(symbols, "Colour")
		assert colour["kind"] == Symbols.Enum
		assert Named(Children(colour), "Red")["kind"] == Symbols.EnumMember

	[Test]
	def ReportsAnInterfaceAndAStruct():
		symbols = SymbolsOf("interface IGreeter:\n\tpass\n\nstruct Point:\n\tx as int\n")
		assert Named(symbols, "IGreeter")["kind"] == Symbols.Interface
		assert Named(symbols, "Point")["kind"] == Symbols.Struct

	[Test]
	def NestsAClassInsideAClass():
		symbols = SymbolsOf("class Outer:\n\tclass Inner:\n\t\tpass\n")
		assert Named(Children(Named(symbols, "Outer")), "Inner")["kind"] == Symbols.Class

	[Test]
	def SelectsTheNameAndCoversTheBody():
		greeter = Named(SymbolsOf("class Greeter:\n\tdef Hello():\n\t\tpass\n"), "Greeter")
		span = greeter["range"] as Dictionary[of string, object]
		selection = greeter["selectionRange"] as Dictionary[of string, object]
		start = selection["start"] as Dictionary[of string, object]
		finish = selection["end"] as Dictionary[of string, object]
		# "class Greeter:" puts the name at character 6.
		assert start["line"] == 0L
		assert start["character"] == 6L
		assert finish["character"] == 13L
		# The whole definition reaches the last line.
		lastLine = cast(long, (span["end"] as Dictionary[of string, object])["line"])
		assert lastLine >= 2L

	[Test]
	def KeepsTheOutlineBelowAHalfTypedCall():
		# Without repair the unclosed paren hides every newline after it and
		# Second is lost.
		text = "class First:\n\tdef a():\n\t\tx = f(\n\nclass Second:\n\tdef b():\n\t\tpass\n"
		symbols = SymbolsOf(text)
		assert Named(symbols, "First") is not null
		assert Named(symbols, "Second") is not null
		assert Named(Children(Named(symbols, "Second")), "b") is not null

	[Test]
	def KeepsTheOutlineAfterAStrayCloser():
		text = "class First:\n\tdef a():\n\t\tx = 1)\n\nclass Second:\n\tdef b():\n\t\tpass\n"
		symbols = SymbolsOf(text)
		assert Named(symbols, "First") is not null
		assert Named(symbols, "Second") is not null

	[Test]
	def StillReportsWhatParsedFromABrokenFile():
		symbols = SymbolsOf("class Greeter:\n\tdef Hello():\n\t\tpass\n\nclass = 2\n")
		assert Named(symbols, "Greeter") is not null
