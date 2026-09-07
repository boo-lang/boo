namespace Boo.Lang.Lsp.Tests.Server

import System.Collections.Generic
import System.IO
import System.Text
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Tests
import Boo.Lang.Lsp.Server
import System.Text.Json.Nodes

class ServerFixture:
"""
Runs a whole server session over a pair of memory streams.

The debounce is shortened so a test does not sit out the one a real editor
is given.
"""

	static final Debounce = 20
	static final Handshake = '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}'

	protected _output as MemoryStream
	protected _server as LanguageServer

	protected def Serve(*messages as (string)) as int:
	"""Serves the handshake and then these messages, and returns the exit code."""
		all = List[of string]()
		all.Add(Handshake)
		all.AddRange(messages)
		return ServeAlone(*all.ToArray())

	protected def ServeAlone(*messages as (string)) as int:
	"""The same without the handshake, for tests about the handshake itself."""
		encoding = UTF8Encoding(false)
		wire = StringBuilder()
		for message in messages:
			wire.Append("Content-Length: ${encoding.GetByteCount(message)}\r\n\r\n").Append(message)
		_output = MemoryStream()
		input = MemoryStream(encoding.GetBytes(wire.ToString()))
		_server = LanguageServer(MessageStream(input, _output), Debounce)
		return _server.Run()

	protected def Replies() as List[of JsonObject]:
	"""Every message the server wrote back, in order."""
		replies = List[of JsonObject]()
		stream = MessageStream(MemoryStream(_output.ToArray()), MemoryStream())
		while true:
			message = stream.Read()
			break if message is null
			replies.Add(JsonCodec.Parse(message) as JsonObject)
		return replies

	protected def ReplyTo(id as long) as JsonObject:
		for reply in Replies():
			continue unless reply.ContainsKey("id")
			return reply if Fields.Number(reply, "id", 0) == id
		return null

	protected def Notified(method as string) as List[of JsonObject]:
	"""The params of every notification the server sent with this method."""
		sent = List[of JsonObject]()
		for reply in Replies():
			sent.Add(Fields.Map(reply, "params")) if Fields.Text(reply, "method") == method
		return sent
