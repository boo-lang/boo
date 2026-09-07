namespace Boo.Lang.Lsp.Tests.Workspace

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Workspace
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class DiagnosticsTestFixture:
"""
A compiler error carries where it starts but not where it ends, so the end of
the range is worked out from the document.
"""

	private def Document(text as string):
		return TextDocument("file:///a.boo", "boo", 1, text)

	private def RangeOf(document as TextDocument, line as int, column as int):
		error = CompilerError("BCE0005", LexicalInfo("file:///a.boo", line, column), "nosuchname")
		diagnostic = Diagnostic.FromError(document, error)
		return Fields.Map(diagnostic, "range")

	private def Start(span as JsonObject):
		return Fields.Map(span, "start")

	private def End(span as JsonObject):
		return Fields.Map(span, "end")

	[Test]
	def StartsWhereTheCompilerSaysMinusOne():
		start = Start(RangeOf(Document("print nosuchname\n"), 1, 7))
		assert Fields.Number(start, "line", 0) == 0
		assert Fields.Number(start, "character", 0) == 6

	[Test]
	def EndsAtTheEndOfTheWord():
		assert Fields.Number(End(RangeOf(Document("print nosuchname\n"), 1, 7)), "character", 0) == 16

	[Test]
	def StopsTheWordAtPunctuation():
		assert Fields.Number(End(RangeOf(Document("print foo.bar\n"), 1, 7)), "character", 0) == 9

	[Test]
	def CoversOneCharacterWhenTheWordIsEmpty():
		assert Fields.Number(End(RangeOf(Document("x = (\n"), 1, 5)), "character", 0) == 5

	[Test]
	def CollapsesToTheStartWhenTheCompilerNeverSetALocation():
		span = RangeOf(Document("x = 1\n"), -1, -1)
		assert Fields.Number(Start(span), "line", 0) == 0
		assert Fields.Number(Start(span), "character", 0) == 0
		assert Fields.Number(End(span), "character", 0) == 0

	[Test]
	def MarksAWarningAsSeverityTwo():
		warning = CompilerWarning("BCW0016", LexicalInfo("file:///a.boo", 1, 1), "System.Collections")
		diagnostic = Diagnostic.FromWarning(Document("import System.Collections\n"), warning)
		assert Fields.Number(diagnostic, "severity", 0) == 2
		assert Fields.Text(diagnostic, "code") == "BCW0016"

	[Test]
	def UnderlinesTheWordOnAnIndentedLine():
		# The compiler reports column 16 here; the word starts at character 12.
		span = RangeOf(Document("def f(x as int):\n\treturn x + nmae\n"), 2, 16)
		assert Fields.Number(Start(span), "line", 0) == 1
		assert Fields.Number(Start(span), "character", 0) == 12
		assert Fields.Number(End(span), "character", 0) == 16

	[Test]
	def CarriesTheCodeAndTheMessage():
		error = CompilerError("BCE0005", LexicalInfo("file:///a.boo", 1, 7), "nosuchname")
		diagnostic = Diagnostic.FromError(Document("print nosuchname\n"), error)
		assert Fields.Text(diagnostic, "code") == "BCE0005"
		assert Fields.Text(diagnostic, "source") == "boo"
		assert Fields.Number(diagnostic, "severity", 0) == 1
		assert Fields.Text(diagnostic, "message").Length > 0

	[Test]
	def UnderlinesAWholeDottedNameTheMessageNames():
	"""
	A namespace or assembly is reported at the first segment, and the
	whole of what could not be found is what the reader has to see.
	"""
		error = CompilerError("BCE0021", LexicalInfo("file:///a.boo", 1, 8), "System.Web.UI")
		diagnostic = Diagnostic.FromError(Document("import System.Web.UI\n"), error)
		span = Fields.Map(diagnostic, "range")
		assert Fields.Number(End(span), "character", 0) == 20, Fields.Text(diagnostic, "message")

	[Test]
	def StopsAtTheNameTheMessageActuallyBlames():
	"""
	Only the identifier is unknown in Application.Run, so the call after
	it stays out of the squiggle.
	"""
		error = CompilerError("BCE0005", LexicalInfo("file:///a.boo", 1, 1), "Application")
		diagnostic = Diagnostic.FromError(Document("Application.Run(f)\n"), error)
		span = Fields.Map(diagnostic, "range")
		assert Fields.Number(End(span), "character", 0) == 11, Fields.Text(diagnostic, "message")

	[Test]
	def MarksSomethingNeverUsedAsUnnecessary():
	"""The client fades what it is told is unnecessary rather than drawing it."""
		warning = CompilerWarning("BCW0016", LexicalInfo("file:///a.boo", 1, 8), "System.Xml")
		diagnostic = Diagnostic.FromWarning(Document("import System.Xml\n"), warning)
		tags = Fields.Items(diagnostic, "tags")
		assert tags is not null and tags.Count == 1
		assert Fields.Value[of int](tags[0]) == Diagnostic.Unnecessary

	[Test]
	def LeavesAnOrdinaryReportUntagged():
		error = CompilerError("BCE0005", LexicalInfo("file:///a.boo", 1, 7), "nosuchname")
		assert not Diagnostic.FromError(Document("print nosuchname\n"), error).ContainsKey("tags")
