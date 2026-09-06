namespace Boo.Lang.Lsp.Tests.Workspace

import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class AnalyzerTestFixture:

	analyzer as Analyzer

	[SetUp]
	def Setup():
		analyzer = Analyzer()

	private def Document(text as string):
		return TextDocument("file:///a.boo", "boo", 1, text)

	private def Codes(diagnostics as List[of object]):
		codes = List[of string]()
		for diagnostic in diagnostics:
			codes.Add(cast(string, (diagnostic as Dictionary[of string, object])["code"]))
		return codes

	[Test]
	def FindsNothingWrongWithACleanFile():
		assert analyzer.Parse(Document("x = 1\nprint x\n")).Count == 0

	[Test]
	def ReportsASyntaxError():
		diagnostics = analyzer.Parse(Document("x = 1\nclass = 2\n"))
		assert diagnostics.Count > 0
		first = diagnostics[0] as Dictionary[of string, object]
		span = first["range"] as Dictionary[of string, object]
		start = span["start"] as Dictionary[of string, object]
		assert start["line"] == 1L
		assert first["severity"] == 1L
		assert first["source"] == "boo"

	[Test]
	def SaysNothingAboutTypesWhenOnlyParsing():
		# Parsing cannot know that nosuchname is unknown; binding does.
		assert analyzer.Parse(Document("print nosuchname\n")).Count == 0

	[Test]
	def ReportsAnUnknownNameWhenBinding():
		assert "BCE0005" in Codes(analyzer.Bind(Document("print nosuchname\n")))

	[Test]
	def SaysNothingAboutAnUnusedImportWhenBinding():
		# BCW0016 and its kind come out of the full Compile pipeline, which
		# emits IL. Binding alone reports errors and no warnings at all.
		assert analyzer.Bind(Document("import System.Collections\nx = 1\nprint x\n")).Count == 0

	[Test]
	def SurvivesAnUnclosedBracket():
		# The parser loses the rest of the file here, which is the M5 problem;
		# what matters now is that it still answers.
		assert analyzer.Parse(Document("def f():\n\tx = g(\n\ny = 2\n")).Count > 0

	[Test]
	def SurvivesAnEmptyDocument():
		assert analyzer.Parse(Document("")).Count == 0
