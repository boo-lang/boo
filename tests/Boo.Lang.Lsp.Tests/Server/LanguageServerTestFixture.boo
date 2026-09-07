namespace Boo.Lang.Lsp.Tests.Server

import System
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Server
import System.Text.Json.Nodes

[TestFixture]
class LanguageServerTestFixture(ServerFixture):
"""Runs a whole server session over a fake transport."""


	private static final Initialize = '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"processId":null,"capabilities":{}}}'
	private static final Initialized = '{"jsonrpc":"2.0","method":"initialized","params":{}}'
	private static final Shutdown = '{"jsonrpc":"2.0","id":2,"method":"shutdown"}'
	private static final Exit = '{"jsonrpc":"2.0","method":"exit"}'

	[Test]
	def AnswersInitializeWithItsNameAndCapabilities():
		ServeAlone(Initialize)
		result = Replies()[0]["result"] as JsonObject
		info = Fields.Map(result, "serverInfo")
		assert Fields.Text(info, "name") == ServerInfo.Name
		assert Fields.Text(info, "version") == ServerInfo.Version
		assert result.ContainsKey("capabilities")

	[Test]
	def CompletesTheWholeHandshake():
		exitCode = ServeAlone(Initialize, Initialized, Shutdown, Exit)
		replies = Replies()
		assert replies.Count == 2
		assert Fields.Number(replies[0], "id", 0) == 1
		assert Fields.Number(replies[1], "id", 0) == 2
		assert replies[1]["result"] is null
		assert exitCode == 0

	[Test]
	def ExitsWithOneWhenShutdownNeverCame():
		assert ServeAlone(Initialize, Exit) == 1

	[Test]
	def RefusesWorkBeforeInitialize():
		ServeAlone('{"jsonrpc":"2.0","id":1,"method":"shutdown"}')
		error = Replies()[0]["error"] as JsonObject
		assert Fields.Number(error, "code", 0) == JsonRpc.ServerNotInitialized

	[Test]
	def RefusesASecondInitialize():
		ServeAlone(Initialize, Initialize)
		error = Replies()[1]["error"] as JsonObject
		assert Fields.Number(error, "code", 0) == JsonRpc.InvalidRequest

	[Test]
	def RefusesWorkAfterShutdown():
		ServeAlone(Initialize, Shutdown, '{"jsonrpc":"2.0","id":3,"method":"shutdown"}')
		error = Replies()[2]["error"] as JsonObject
		assert Fields.Number(error, "code", 0) == JsonRpc.InvalidRequest

	[Test]
	def StopsReadingAfterExit():
		ServeAlone(Initialize, Shutdown, Exit, '{"jsonrpc":"2.0","id":9,"method":"initialize"}')
		assert Replies().Count == 2
