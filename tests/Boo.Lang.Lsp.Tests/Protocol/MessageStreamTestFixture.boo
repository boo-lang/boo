namespace Boo.Lang.Lsp.Tests.Protocol

import System
import System.IO
import System.Text
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Protocol

[TestFixture]
class MessageStreamTestFixture:

	private def Bytes(text as string):
		return UTF8Encoding(false).GetBytes(text)

	private def Text(bytes as (byte)):
		return UTF8Encoding(false).GetString(bytes)

	private def Reading(wire as string):
		return MessageStream(MemoryStream(Bytes(wire)), MemoryStream())

	[Test]
	def WritesTheHeaderAndTheBody():
		output = MemoryStream()
		MessageStream(MemoryStream(), output).Write('{"id":1}')
		assert Text(output.ToArray()) == "Content-Length: 8\r\n\r\n" + '{"id":1}'

	[Test]
	def CountsHeaderLengthInBytesNotCharacters():
		output = MemoryStream()
		MessageStream(MemoryStream(), output).Write('{"t":"café"}')
		wire = Text(output.ToArray())
		assert wire.StartsWith("Content-Length: 13\r\n\r\n")

	[Test]
	def ReadsASingleMessage():
		assert Reading("Content-Length: 8\r\n\r\n" + '{"id":1}').Read() == '{"id":1}'

	[Test]
	def ReadsTwoMessagesBackToBack():
		stream = Reading("Content-Length: 8\r\n\r\n" + '{"id":1}' + "Content-Length: 8\r\n\r\n" + '{"id":2}')
		assert stream.Read() == '{"id":1}'
		assert stream.Read() == '{"id":2}'

	[Test]
	def ReadsAMessageWithNonAsciiContent():
		body = '{"t":"café"}'
		assert Reading("Content-Length: 13\r\n\r\n" + body).Read() == body

	[Test]
	def IgnoresOtherHeaders():
		wire = "Content-Type: application/vscode-jsonrpc; charset=utf-8\r\nContent-Length: 8\r\n\r\n" + '{"id":1}'
		assert Reading(wire).Read() == '{"id":1}'

	[Test]
	def AcceptsAHeaderNameInAnyCase():
		assert Reading("content-length: 8\r\n\r\n" + '{"id":1}').Read() == '{"id":1}'

	[Test]
	def ReturnsNullAtEndOfStream():
		assert Reading("").Read() is null

	[Test]
	def ReturnsNullWhenTheStreamEndsBetweenMessages():
		stream = Reading("Content-Length: 8\r\n\r\n" + '{"id":1}')
		assert stream.Read() == '{"id":1}'
		assert stream.Read() is null

	[Test]
	def RejectsHeadersWithoutAContentLength():
		e = Assert.Throws[of ProtocolError]({ Reading("Content-Type: text/plain\r\n\r\n").Read() })
		assert "Content-Length" in e.Message

	[Test]
	def RejectsATruncatedBody():
		Assert.Throws[of ProtocolError]({ Reading("Content-Length: 8\r\n\r\n" + '{"id"').Read() })
