namespace Boo.Lang.Lsp.Tests.Workspace

import System.Collections.Generic
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Workspace

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
		return diagnostic["range"] as Dictionary[of string, object]

	private def Start(span as Dictionary[of string, object]):
		return span["start"] as Dictionary[of string, object]

	private def End(span as Dictionary[of string, object]):
		return span["end"] as Dictionary[of string, object]

	[Test]
	def StartsWhereTheCompilerSaysMinusOne():
		start = Start(RangeOf(Document("print nosuchname\n"), 1, 7))
		assert start["line"] == 0L
		assert start["character"] == 6L

	[Test]
	def EndsAtTheEndOfTheWord():
		assert End(RangeOf(Document("print nosuchname\n"), 1, 7))["character"] == 16L

	[Test]
	def StopsTheWordAtPunctuation():
		assert End(RangeOf(Document("print foo.bar\n"), 1, 7))["character"] == 9L

	[Test]
	def CoversOneCharacterWhenTheWordIsEmpty():
		assert End(RangeOf(Document("x = (\n"), 1, 5))["character"] == 5L

	[Test]
	def CollapsesToTheStartWhenTheCompilerNeverSetALocation():
		span = RangeOf(Document("x = 1\n"), -1, -1)
		assert Start(span)["line"] == 0L
		assert Start(span)["character"] == 0L
		assert End(span)["character"] == 0L

	[Test]
	def MarksAWarningAsSeverityTwo():
		warning = CompilerWarning("BCW0016", LexicalInfo("file:///a.boo", 1, 1), "System.Collections")
		diagnostic = Diagnostic.FromWarning(Document("import System.Collections\n"), warning)
		assert diagnostic["severity"] == 2
		assert diagnostic["code"] == "BCW0016"

	[Test]
	def UnderlinesTheWordOnAnIndentedLine():
		# The compiler reports column 16 here; the word starts at character 12.
		span = RangeOf(Document("def f(x as int):\n\treturn x + nmae\n"), 2, 16)
		assert Start(span)["line"] == 1L
		assert Start(span)["character"] == 12L
		assert End(span)["character"] == 16L

	[Test]
	def CarriesTheCodeAndTheMessage():
		error = CompilerError("BCE0005", LexicalInfo("file:///a.boo", 1, 7), "nosuchname")
		diagnostic = Diagnostic.FromError(Document("print nosuchname\n"), error)
		assert diagnostic["code"] == "BCE0005"
		assert diagnostic["source"] == "boo"
		assert diagnostic["severity"] == 1
		assert cast(string, diagnostic["message"]).Length > 0

	[Test]
	def UnderlinesAWholeDottedNameTheMessageNames():
	"""
	A namespace or assembly is reported at the first segment, and the
	whole of what could not be found is what the reader has to see.
	"""
		error = CompilerError("BCE0021", LexicalInfo("file:///a.boo", 1, 8), "System.Web.UI")
		diagnostic = Diagnostic.FromError(Document("import System.Web.UI\n"), error)
		span = diagnostic["range"] as Dictionary[of string, object]
		assert End(span)["character"] == 20L, diagnostic["message"]

	[Test]
	def StopsAtTheNameTheMessageActuallyBlames():
	"""
	Only the identifier is unknown in Application.Run, so the call after
	it stays out of the squiggle.
	"""
		error = CompilerError("BCE0005", LexicalInfo("file:///a.boo", 1, 1), "Application")
		diagnostic = Diagnostic.FromError(Document("Application.Run(f)\n"), error)
		span = diagnostic["range"] as Dictionary[of string, object]
		assert End(span)["character"] == 11L, diagnostic["message"]

	[Test]
	def MarksSomethingNeverUsedAsUnnecessary():
	"""The client fades what it is told is unnecessary rather than drawing it."""
		warning = CompilerWarning("BCW0016", LexicalInfo("file:///a.boo", 1, 8), "System.Xml")
		diagnostic = Diagnostic.FromWarning(Document("import System.Xml\n"), warning)
		tags = diagnostic["tags"] as List[of object]
		assert tags is not null and tags.Count == 1
		assert tags[0] == Diagnostic.Unnecessary

	[Test]
	def LeavesAnOrdinaryReportUntagged():
		error = CompilerError("BCE0005", LexicalInfo("file:///a.boo", 1, 7), "nosuchname")
		assert not Diagnostic.FromError(Document("print nosuchname\n"), error).ContainsKey("tags")
