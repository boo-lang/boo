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
class LanguageServerTestFixture:
"""Runs a whole server session over a fake transport."""

	_output as MemoryStream

	private def Serve(*messages as (string)):
		wire = StringBuilder()
		for message in messages:
			wire.Append("Content-Length: ${UTF8Encoding(false).GetByteCount(message)}\r\n\r\n").Append(message)
		_output = MemoryStream()
		input = MemoryStream(UTF8Encoding(false).GetBytes(wire.ToString()))
		return LanguageServer(MessageStream(input, _output)).Run()

	private def Replies():
		replies = List[of Dictionary[of string, object]]()
		stream = MessageStream(MemoryStream(_output.ToArray()), MemoryStream())
		while true:
			message = stream.Read()
			break if message is null
			replies.Add(JsonCodec.Parse(message) as Dictionary[of string, object])
		return replies

	private static final Initialize = '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"processId":null,"capabilities":{}}}'
	private static final Initialized = '{"jsonrpc":"2.0","method":"initialized","params":{}}'
	private static final Shutdown = '{"jsonrpc":"2.0","id":2,"method":"shutdown"}'
	private static final Exit = '{"jsonrpc":"2.0","method":"exit"}'

	[Test]
	def AnswersInitializeWithItsNameAndCapabilities():
		Serve(Initialize)
		result = Replies()[0]["result"] as Dictionary[of string, object]
		info = result["serverInfo"] as Dictionary[of string, object]
		assert info["name"] == ServerInfo.Name
		assert info["version"] == ServerInfo.Version
		assert result.ContainsKey("capabilities")

	[Test]
	def CompletesTheWholeHandshake():
		exitCode = Serve(Initialize, Initialized, Shutdown, Exit)
		replies = Replies()
		assert replies.Count == 2
		assert replies[0]["id"] == 1L
		assert replies[1]["id"] == 2L
		assert replies[1]["result"] is null
		assert exitCode == 0

	[Test]
	def ExitsWithOneWhenShutdownNeverCame():
		assert Serve(Initialize, Exit) == 1

	[Test]
	def RefusesWorkBeforeInitialize():
		Serve('{"jsonrpc":"2.0","id":1,"method":"shutdown"}')
		error = Replies()[0]["error"] as Dictionary[of string, object]
		assert error["code"] == cast(long, JsonRpc.ServerNotInitialized)

	[Test]
	def RefusesASecondInitialize():
		Serve(Initialize, Initialize)
		error = Replies()[1]["error"] as Dictionary[of string, object]
		assert error["code"] == cast(long, JsonRpc.InvalidRequest)

	[Test]
	def RefusesWorkAfterShutdown():
		Serve(Initialize, Shutdown, '{"jsonrpc":"2.0","id":3,"method":"shutdown"}')
		error = Replies()[2]["error"] as Dictionary[of string, object]
		assert error["code"] == cast(long, JsonRpc.InvalidRequest)

	[Test]
	def StopsReadingAfterExit():
		Serve(Initialize, Shutdown, Exit, '{"jsonrpc":"2.0","id":9,"method":"initialize"}')
		assert Replies().Count == 2
