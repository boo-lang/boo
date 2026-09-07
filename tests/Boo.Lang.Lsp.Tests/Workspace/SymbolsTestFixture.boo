namespace Boo.Lang.Lsp.Tests.Workspace

import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Workspace
import System.Text.Json.Nodes

[TestFixture]
class SymbolsTestFixture:

	analyzer as Analyzer

	[SetUp]
	def Setup():
		analyzer = Analyzer()

	private def SymbolsOf(text as string):
		document = TextDocument("file:///a.boo", "boo", 1, text)
		return Symbols.Of(document, analyzer.ParseTree(document))

	private def Named(symbols as JsonArray, name as string) as JsonObject:
		for symbol as JsonObject in symbols:
			return symbol if Fields.Text(symbol, "name") == name
		return null

	private def Children(symbol as JsonObject):
		return Fields.Items(symbol, "children")

	[Test]
	def ReportsNothingForAnEmptyDocument():
		assert SymbolsOf("").Count == 0

	[Test]
	def ReportsAClass():
		greeter = Named(SymbolsOf("class Greeter:\n\tpass\n"), "Greeter")
		assert greeter is not null
		assert Fields.Number(greeter, "kind", 0) == Symbols.Class

	[Test]
	def NestsMembersUnderTheirType():
		symbols = SymbolsOf("class Greeter:\n\tdef Hello():\n\t\tpass\n")
		hello = Named(Children(Named(symbols, "Greeter")), "Hello")
		assert hello is not null
		assert Fields.Number(hello, "kind", 0) == Symbols.Method

	[Test]
	def ReportsAModuleLevelDefAsAFunction():
		top = Named(SymbolsOf("def Hello():\n\tpass\n"), "Hello")
		assert Fields.Number(top, "kind", 0) == Symbols.Function

	[Test]
	def ReportsADefThatFollowsModuleLevelCode():
		# Such a def parses as a declaration in the module's globals rather
		# than as a member, and is easy to lose.
		symbols = SymbolsOf("def Before():\n\tpass\n\nx = 1\n\ndef After():\n\tpass\n")
		assert Named(symbols, "Before") is not null
		assert Named(symbols, "After") is not null
		assert Fields.Number(Named(symbols, "After"), "kind", 0) == Symbols.Function

	[Test]
	def SelectsTheNameOfADefBelowModuleLevelCode():
		# Such a def is pointed at its keyword, not its name.
		symbols = SymbolsOf("x = 1\n\ndef After():\n\tpass\n")
		selection = Named(symbols, "After")["selectionRange"] as JsonObject
		start = Fields.Map(selection, "start")
		finish = Fields.Map(selection, "end")
		assert Fields.Number(start, "character", 0) == 4
		assert Fields.Number(finish, "character", 0) == 9

	[Test]
	def ReportsFieldsAndProperties():
		symbols = SymbolsOf("class Greeter:\n\tname as string\n\n\tGreeting:\n\t\tget: return 'hi'\n")
		members = Children(Named(symbols, "Greeter"))
		assert Fields.Number(Named(members, "name"), "kind", 0) == Symbols.Field
		assert Fields.Number(Named(members, "Greeting"), "kind", 0) == Symbols.Property

	[Test]
	def ReportsAnEnumAndItsMembers():
		symbols = SymbolsOf("enum Colour:\n\tRed\n\tGreen\n")
		colour = Named(symbols, "Colour")
		assert Fields.Number(colour, "kind", 0) == Symbols.Enum
		assert Fields.Number(Named(Children(colour), "Red"), "kind", 0) == Symbols.EnumMember

	[Test]
	def ReportsAnInterfaceAndAStruct():
		symbols = SymbolsOf("interface IGreeter:\n\tpass\n\nstruct Point:\n\tx as int\n")
		assert Fields.Number(Named(symbols, "IGreeter"), "kind", 0) == Symbols.Interface
		assert Fields.Number(Named(symbols, "Point"), "kind", 0) == Symbols.Struct

	[Test]
	def NestsAClassInsideAClass():
		symbols = SymbolsOf("class Outer:\n\tclass Inner:\n\t\tpass\n")
		assert Fields.Number(Named(Children(Named(symbols, "Outer")), "Inner"), "kind", 0) == Symbols.Class

	[Test]
	def SelectsTheNameAndCoversTheBody():
		greeter = Named(SymbolsOf("class Greeter:\n\tdef Hello():\n\t\tpass\n"), "Greeter")
		span = Fields.Map(greeter, "range")
		selection = Fields.Map(greeter, "selectionRange")
		start = Fields.Map(selection, "start")
		finish = Fields.Map(selection, "end")
		# "class Greeter:" puts the name at character 6.
		assert Fields.Number(start, "line", 0) == 0
		assert Fields.Number(start, "character", 0) == 6
		assert Fields.Number(finish, "character", 0) == 13
		# The whole definition reaches the last line.
		lastLine = Fields.Number(Fields.Map(span, "end"), "line", 0)
		assert lastLine >= 2

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
