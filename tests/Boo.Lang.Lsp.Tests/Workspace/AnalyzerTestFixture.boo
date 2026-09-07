namespace Boo.Lang.Lsp.Tests.Workspace

import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Workspace
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class AnalyzerTestFixture:

	analyzer as Analyzer

	[SetUp]
	def Setup():
		analyzer = Analyzer()

	private def Document(text as string):
		return TextDocument("file:///a.boo", "boo", 1, text)

	private def Codes(diagnostics as JsonArray):
		codes = List[of string]()
		for diagnostic in diagnostics:
			codes.Add(Fields.Text(diagnostic, "code"))
		return codes

	[Test]
	def FindsNothingWrongWithACleanFile():
		assert analyzer.Parse(Document("x = 1\nprint x\n")).Count == 0

	[Test]
	def ReportsASyntaxError():
		diagnostics = analyzer.Parse(Document("x = 1\nclass = 2\n"))
		assert diagnostics.Count > 0
		first = diagnostics[0] as JsonObject
		span = Fields.Map(first, "range")
		start = Fields.Map(span, "start")
		assert Fields.Number(start, "line", 0) == 1
		assert Fields.Number(first, "severity", 0) == 1
		assert Fields.Text(first, "source") == "boo"

	[Test]
	def SaysNothingAboutTypesWhenOnlyParsing():
		# Parsing cannot know that nosuchname is unknown; binding does.
		assert analyzer.Parse(Document("print nosuchname\n")).Count == 0

	[Test]
	def ReportsAnUnknownNameWhenBinding():
		assert "BCE0005" in Codes(analyzer.Bind(Document("print nosuchname\n")))

	[Test]
	def SurvivesAnUnclosedBracket():
		# The parser loses the rest of the file here, which is the M5 problem;
		# what matters now is that it still answers.
		assert analyzer.Parse(Document("def f():\n\tx = g(\n\ny = 2\n")).Count > 0

	[Test]
	def SurvivesAnEmptyDocument():
		assert analyzer.Parse(Document("")).Count == 0

	[Test]
	def BindsAgainWhenTheTextChangesUnderTheSameVersion():
	"""
	A bind is kept between requests, and the text is part of what it is
	kept against: an editor need not have moved the version on.
	"""
		assert analyzer.Bind(Document("x = 1\nprint x\n")).Count == 0
		assert analyzer.Bind(Document("print nosuchname\n")).Count > 0

	[Test]
	def KeepsTheSameBindForTheSameDocument():
		document = Document("y = 2\nprint y\n")
		first = analyzer.Bound(document)
		assert first is not null
		assert analyzer.Bound(document) is first

	[Test]
	def ReportsAnImportNothingUses():
		assert "BCW0016" in Codes(analyzer.Bind(Document("import System.Xml\n\nprint 1\n")))

	[Test]
	def SaysNothingAboutAnImportInUse():
		codes = Codes(analyzer.Bind(Document("import System.IO\n\nprint Path.GetTempPath()\n")))
		assert "BCW0016" not in codes, string.Join(",", codes.ToArray())

	[Test]
	def SaysNothingAboutAPrivateMemberInUse():
	"""
	Whether a member is used is not settled by the time names are bound,
	so the report on one is not worth passing on.
	"""
		text = "class Greeter:\n\tdef Hello():\n\t\treturn Twice()\n\n\tprivate def Twice():\n\t\treturn 2\n"
		assert "BCW0014" not in Codes(analyzer.Bind(Document(text)))

	private def Severities(diagnostics as JsonArray) as List[of int]:
		found = List[of int]()
		for diagnostic in diagnostics:
			found.Add(Fields.Number(diagnostic, "severity", 0))
		return found

	private def MessageOf(diagnostic as object) as string:
		return Fields.Text(diagnostic, "message")

	[Test]
	def StopsReportingPastTheLimit():
	"""
	One unresolved type can leave the compiler repeating itself thousands of
	times, which costs memory here and is unreadable at the other end.
	"""
		text = System.Text.StringBuilder()
		for i in range(Analyzer.Limit + 60):
			text.Append("print nosuchname").Append(i).Append("\n")
		diagnostics = analyzer.Bind(Document(text.ToString()))
		assert diagnostics.Count == Analyzer.Limit + 1, "reported ${diagnostics.Count}"

	[Test]
	def SaysHowManyItLeftOut():
		text = System.Text.StringBuilder()
		for i in range(Analyzer.Limit + 60):
			text.Append("print nosuchname").Append(i).Append("\n")
		diagnostics = analyzer.Bind(Document(text.ToString()))
		last = diagnostics[diagnostics.Count - 1]
		assert "60 more problems here are not shown." == MessageOf(last), MessageOf(last)
		assert Diagnostic.Information == Fields.Number(last, "severity", 0)

	[Test]
	def SaysNothingAboutWhatItDidNotLeaveOut():
		diagnostics = analyzer.Bind(Document("print nosuchname\n"))
		for diagnostic in diagnostics:
			assert "not shown" not in MessageOf(diagnostic), MessageOf(diagnostic)

	[Test]
	def SaysWhatItCanWhenAStepFallsOver():
	"""
	A step that raises is reported with its exception text, which names an
	array index and a class nobody reading Boo has heard of.
	"""
		built = cast(JsonObject, Diagnostic.Internal("BCE0011"))
		assert "Boo could not analyse this file together with the files beside it." == Fields.Text(built, "message"), Fields.Text(built, "message")
		assert "BCE0011" == Fields.Text(built, "code")
		assert Diagnostic.Error == Fields.Number(built, "severity", 0)
