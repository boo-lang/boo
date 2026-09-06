namespace Boo.Lang.Lsp.Tests.Server

import System
import System.Collections.Generic
import System.IO
import System.Text
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Server

[TestFixture]
class TextDocumentSyncTestFixture:
"""Drives the sync notifications through a whole server session."""

	_server as LanguageServer

	private def Serve(*messages as (string)):
		wire = StringBuilder()
		all = List[of string]()
		all.Add('{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}')
		all.AddRange(messages)
		for message in all:
			wire.Append("Content-Length: ${UTF8Encoding(false).GetByteCount(message)}\r\n\r\n").Append(message)
		input = MemoryStream(UTF8Encoding(false).GetBytes(wire.ToString()))
		_server = LanguageServer(MessageStream(input, MemoryStream()))
		_server.Run()
		return _server.Documents

	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"x = 1"}}}'

	[Test]
	def OpensADocument():
		documents = Serve(Opened)
		assert documents.Count == 1
		assert documents.Get("file:///a.boo").Text == "x = 1"

	[Test]
	def AppliesAFullTextChange():
		changed = '{"jsonrpc":"2.0","method":"textDocument/didChange","params":{"textDocument":{"uri":"file:///a.boo","version":2},"contentChanges":[{"text":"x = 2"}]}}'
		document = Serve(Opened, changed).Get("file:///a.boo")
		assert document.Text == "x = 2"
		assert document.Version == 2

	[Test]
	def TakesTheLastOfSeveralChanges():
		changed = '{"jsonrpc":"2.0","method":"textDocument/didChange","params":{"textDocument":{"uri":"file:///a.boo","version":3},"contentChanges":[{"text":"first"},{"text":"last"}]}}'
		assert Serve(Opened, changed).Get("file:///a.boo").Text == "last"

	[Test]
	def ClosesADocument():
		closed = '{"jsonrpc":"2.0","method":"textDocument/didClose","params":{"textDocument":{"uri":"file:///a.boo"}}}'
		assert Serve(Opened, closed).Count == 0

	[Test]
	def LeavesTheDocumentAloneOnSave():
		saved = '{"jsonrpc":"2.0","method":"textDocument/didSave","params":{"textDocument":{"uri":"file:///a.boo"}}}'
		assert Serve(Opened, saved).Get("file:///a.boo").Text == "x = 1"

	[Test]
	def SurvivesAChangeForADocumentThatWasNeverOpened():
		changed = '{"jsonrpc":"2.0","method":"textDocument/didChange","params":{"textDocument":{"uri":"file:///gone.boo","version":2},"contentChanges":[{"text":"x"}]}}'
		assert Serve(changed).Count == 0

	[Test]
	def AdvertisesFullTextSync():
		output = MemoryStream()
		message = '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}'
		input = MemoryStream(UTF8Encoding(false).GetBytes("Content-Length: ${UTF8Encoding(false).GetByteCount(message)}\r\n\r\n" + message))
		LanguageServer(MessageStream(input, output)).Run()
		reply = JsonCodec.Parse(MessageStream(MemoryStream(output.ToArray()), MemoryStream()).Read()) as Dictionary[of string, object]
		result = reply["result"] as Dictionary[of string, object]
		capabilities = result["capabilities"] as Dictionary[of string, object]
		sync = capabilities["textDocumentSync"] as Dictionary[of string, object]
		assert sync["openClose"] == true
		assert sync["change"] == 1L
