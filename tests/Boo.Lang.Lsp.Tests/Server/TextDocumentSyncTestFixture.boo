namespace Boo.Lang.Lsp.Tests.Server

import System
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json

[TestFixture]
class TextDocumentSyncTestFixture(ServerFixture):
"""Drives the sync notifications through a whole server session."""


	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"x = 1"}}}'

	private def Syncing(*messages as (string)):
	"""Serves these messages and hands back what the store was left holding."""
		Serve(*messages)
		return _server.Documents

	[Test]
	def OpensADocument():
		documents = Syncing(Opened)
		assert documents.Count == 1
		assert documents.Get("file:///a.boo").Text == "x = 1"

	[Test]
	def AppliesAFullTextChange():
		changed = '{"jsonrpc":"2.0","method":"textDocument/didChange","params":{"textDocument":{"uri":"file:///a.boo","version":2},"contentChanges":[{"text":"x = 2"}]}}'
		document = Syncing(Opened, changed).Get("file:///a.boo")
		assert document.Text == "x = 2"
		assert document.Version == 2

	[Test]
	def TakesTheLastOfSeveralChanges():
		changed = '{"jsonrpc":"2.0","method":"textDocument/didChange","params":{"textDocument":{"uri":"file:///a.boo","version":3},"contentChanges":[{"text":"first"},{"text":"last"}]}}'
		assert Syncing(Opened, changed).Get("file:///a.boo").Text == "last"

	[Test]
	def ClosesADocument():
		closed = '{"jsonrpc":"2.0","method":"textDocument/didClose","params":{"textDocument":{"uri":"file:///a.boo"}}}'
		assert Syncing(Opened, closed).Count == 0

	[Test]
	def LeavesTheDocumentAloneOnSave():
		saved = '{"jsonrpc":"2.0","method":"textDocument/didSave","params":{"textDocument":{"uri":"file:///a.boo"}}}'
		assert Syncing(Opened, saved).Get("file:///a.boo").Text == "x = 1"

	[Test]
	def SurvivesAChangeForADocumentThatWasNeverOpened():
		changed = '{"jsonrpc":"2.0","method":"textDocument/didChange","params":{"textDocument":{"uri":"file:///gone.boo","version":2},"contentChanges":[{"text":"x"}]}}'
		assert Syncing(changed).Count == 0

	[Test]
	def AdvertisesFullTextSync():
		Serve()
		result = Fields.Map(ReplyTo(1L), "result")
		capabilities = Fields.Map(result, "capabilities")
		sync = Fields.Map(capabilities, "textDocumentSync")
		assert Fields.Value[of bool](Fields.Of(sync, "openClose")) == true
		assert Fields.Number(sync, "change", 0) == 1
