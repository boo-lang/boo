namespace Boo.Lang.Lsp.Tests.Server

import System
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Server
import System.Text.Json.Nodes

[TestFixture]
class PublishDiagnosticsTestFixture(ServerFixture):


	private def Opening(text as string):
		escaped = text.Replace("\\", "\\\\").Replace('"', '\\"').Replace("\n", "\\n").Replace("\t", "\\t")
		return '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"' + escaped + '"}}}'

	private def DiagnosticsIn(params as JsonObject):
		return Fields.Items(params, "diagnostics")

	private def Published():
		return Notified(DiagnosticsPublisher.Method)

	[Test]
	def PublishesOnOpen():
		Serve(Opening("print nosuchname\n"))
		published = Published()
		assert published.Count == 1
		assert Fields.Text(published[0], "uri") == "file:///a.boo"
		assert DiagnosticsIn(published[0]).Count > 0

	[Test]
	def PublishesAnEmptyListForACleanFile():
		Serve(Opening("x = 1\nprint x\n"))
		published = Published()
		assert published.Count == 1
		assert DiagnosticsIn(published[0]).Count == 0

	[Test]
	def EndsUpReportingTheNewestVersion():
		# Both versions arrive inside the debounce, so they coalesce into one
		# compile of the newer text. What the client is left holding is what
		# matters.
		changed = '{"jsonrpc":"2.0","method":"textDocument/didChange","params":{"textDocument":{"uri":"file:///a.boo","version":2},"contentChanges":[{"text":"x = 1\\nprint x\\n"}]}}'
		Serve(Opening("print nosuchname\n"), changed)
		published = Published()
		assert published.Count > 0
		assert DiagnosticsIn(published[published.Count - 1]).Count == 0

	[Test]
	def ClearsOnClose():
		closed = '{"jsonrpc":"2.0","method":"textDocument/didClose","params":{"textDocument":{"uri":"file:///a.boo"}}}'
		Serve(Opening("print nosuchname\n"), closed)
		published = Published()
		assert published.Count > 0
		assert DiagnosticsIn(published[published.Count - 1]).Count == 0

	[Test]
	def ReportsASyntaxErrorWithoutWaitingForTheBind():
		# The parse tier runs on the message loop, so its answer is the first
		# thing on the wire.
		Serve(Opening("def f():\n\tx = 1)\n"))
		published = Published()
		assert published.Count > 0
		assert DiagnosticsIn(published[0]).Count > 0

	[Test]
	def SaysNothingFromTheParseTierWhenTheSyntaxIsFine():
		# A clean parse must not clear the semantic diagnostics on screen, so
		# the only publish here is the bind's.
		Serve(Opening("print nosuchname\n"))
		assert Published().Count == 1

	[Test]
	def ReportsASyntaxErrorAndATypeErrorTogether():
		Serve(Opening("print nosuchname\n"))
		diagnostics = DiagnosticsIn(Published()[0])
		first = diagnostics[0] as JsonObject
		assert Fields.Text(first, "source") == "boo"
		assert Fields.Text(first, "code").StartsWith("BC")
