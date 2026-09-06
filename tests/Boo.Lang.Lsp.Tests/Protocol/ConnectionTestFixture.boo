namespace Boo.Lang.Lsp.Tests.Protocol

import System
import System.Collections.Generic
import System.IO
import System.Text
import System.Threading
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol

[TestFixture]
class ConnectionTestFixture:
"""Drives a Connection over a fake transport and reads back what it wrote."""

	_output as MemoryStream
	_connection as Connection

	private def Given(*messages as (string)):
		wire = StringBuilder()
		for message in messages:
			body = UTF8Encoding(false).GetByteCount(message)
			wire.Append("Content-Length: ${body}\r\n\r\n").Append(message)
		_output = MemoryStream()
		input = MemoryStream(UTF8Encoding(false).GetBytes(wire.ToString()))
		_connection = Connection(MessageStream(input, _output))
		return _connection

	private def Replies():
		replies = List[of Dictionary[of string, object]]()
		stream = MessageStream(MemoryStream(_output.ToArray()), MemoryStream())
		while true:
			message = stream.Read()
			break if message is null
			replies.Add(JsonCodec.Parse(message) as Dictionary[of string, object])
		return replies

	private def OnlyReply():
		replies = Replies()
		assert replies.Count == 1
		return replies[0]

	private def ErrorOf(reply as Dictionary[of string, object]):
		return reply["error"] as Dictionary[of string, object]

	[Test]
	def AnswersARequestWithAResult():
		connection = Given('{"jsonrpc":"2.0","id":1,"method":"ping"}')
		connection.OnRequest("ping", { params as object | return "pong" })
		connection.Listen()
		reply = OnlyReply()
		assert reply["jsonrpc"] == "2.0"
		assert reply["id"] == 1L
		assert reply["result"] == "pong"
		assert not reply.ContainsKey("error")

	[Test]
	def EchoesAStringIdVerbatim():
		connection = Given('{"jsonrpc":"2.0","id":"abc","method":"ping"}')
		connection.OnRequest("ping", { params as object | return "pong" })
		connection.Listen()
		assert OnlyReply()["id"] == "abc"

	[Test]
	def PassesTheParamsToTheHandler():
		seen as object
		remember = def(params as object):
			seen = params
			return null
		connection = Given('{"jsonrpc":"2.0","id":1,"method":"ping","params":{"n":3}}')
		connection.OnRequest("ping", remember)
		connection.Listen()
		received = seen as Dictionary[of string, object]
		assert received["n"] == 3L

	[Test]
	def AnswersNothingToANotification():
		called = false
		connection = Given('{"jsonrpc":"2.0","method":"didOpen"}')
		connection.OnNotification("didOpen", { params as object | called = true })
		connection.Listen()
		assert called
		assert Replies().Count == 0

	[Test]
	def IgnoresAnUnknownNotification():
		Given('{"jsonrpc":"2.0","method":"nobody/listens"}').Listen()
		assert Replies().Count == 0

	[Test]
	def ReportsAnUnknownMethod():
		Given('{"jsonrpc":"2.0","id":1,"method":"nope"}').Listen()
		reply = OnlyReply()
		assert reply["id"] == 1L
		assert ErrorOf(reply)["code"] == cast(long, JsonRpc.MethodNotFound)

	[Test]
	def ReportsMalformedJson():
		Given('{"jsonrpc":').Listen()
		reply = OnlyReply()
		assert reply["id"] is null
		assert ErrorOf(reply)["code"] == cast(long, JsonRpc.ParseError)

	[Test]
	def ReportsARequestWithoutAMethod():
		Given('{"jsonrpc":"2.0","id":1}').Listen()
		assert ErrorOf(OnlyReply())["code"] == cast(long, JsonRpc.InvalidRequest)

	[Test]
	def ReportsAHandlerThatRaises():
		connection = Given('{"jsonrpc":"2.0","id":1,"method":"boom"}')
		connection.OnRequest("boom", { params as object | raise InvalidOperationException("no") })
		connection.Listen()
		error = ErrorOf(OnlyReply())
		assert error["code"] == cast(long, JsonRpc.InternalError)
		assert "no" in cast(string, error["message"])

	[Test]
	def KeepsGoingAfterAFailedRequest():
		connection = Given('{"jsonrpc":"2.0","id":1,"method":"boom"}', '{"jsonrpc":"2.0","id":2,"method":"ping"}')
		connection.OnRequest("boom", { params as object | raise InvalidOperationException("no") })
		connection.OnRequest("ping", { params as object | return "pong" })
		connection.Listen()
		replies = Replies()
		assert replies.Count == 2
		assert replies[1]["result"] == "pong"

	[Test]
	def AnswersRequestsInOrder():
		connection = Given('{"jsonrpc":"2.0","id":1,"method":"ping"}', '{"jsonrpc":"2.0","id":2,"method":"ping"}')
		connection.OnRequest("ping", { params as object | return "pong" })
		connection.Listen()
		replies = Replies()
		assert replies[0]["id"] == 1L
		assert replies[1]["id"] == 2L

	[Test]
	def CancelsARequestStillInTheQueue():
		connection = Given(
			'{"jsonrpc":"2.0","method":"$/cancelRequest","params":{"id":2}}',
			'{"jsonrpc":"2.0","id":2,"method":"ping"}')
		connection.OnRequest("ping", { params as object | return "pong" })
		connection.Listen()
		reply = OnlyReply()
		assert reply["id"] == 2L
		assert ErrorOf(reply)["code"] == cast(long, JsonRpc.RequestCancelled)

	[Test]
	def IgnoresAResponseSentByTheClient():
		Given('{"jsonrpc":"2.0","id":1,"result":null}').Listen()
		assert Replies().Count == 0

	[Test]
	def CancelsARequestNamedWhileAnEarlierOneRuns():
	"""
	A client sends the cancellation after the request it names, so it only
	arrives in time if messages are read while a handler is running.
	"""
		connection = Given(
			'{"jsonrpc":"2.0","id":1,"method":"slow"}',
			'{"jsonrpc":"2.0","id":2,"method":"ping"}',
			'{"jsonrpc":"2.0","method":"$/cancelRequest","params":{"id":2}}')
		connection.OnRequest("slow", { params as object | Thread.Sleep(200); return "done" })
		connection.OnRequest("ping", { params as object | return "pong" })
		connection.Listen()
		replies = Replies()
		assert replies.Count == 2, "got ${replies.Count} replies"
		assert replies[1]["id"] == 2L
		assert ErrorOf(replies[1])["code"] == cast(long, JsonRpc.RequestCancelled)
