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
class NavigationTestFixture:

	_output as MemoryStream

	private def Serve(*messages as (string)):
		wire = StringBuilder()
		all = List[of string]()
		all.Add('{"jsonrpc":"2.0","id":1,"method":"initialize","params":{}}')
		all.AddRange(messages)
		for message in all:
			wire.Append("Content-Length: ${UTF8Encoding(false).GetByteCount(message)}\r\n\r\n").Append(message)
		_output = MemoryStream()
		input = MemoryStream(UTF8Encoding(false).GetBytes(wire.ToString()))
		LanguageServer(MessageStream(input, _output), 20).Run()

	private def ReplyTo(id as long) as Dictionary[of string, object]:
		stream = MessageStream(MemoryStream(_output.ToArray()), MemoryStream())
		while true:
			message = stream.Read()
			break if message is null
			parsed = JsonCodec.Parse(message) as Dictionary[of string, object]
			continue unless parsed.ContainsKey("id")
			return parsed if cast(long, parsed["id"]) == id
		return null

	# class Greeter / def Hello(who as string) / g = Greeter() / print g.Hello('x')
	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"class Greeter:\\n\\tdef Hello(who as string) as string:\\n\\t\\treturn who\\n\\ng = Greeter()\\nprint g.Hello(\'x\')\\n"}}}'

	private def Asking(method as string, line as int, character as int):
		return '{"jsonrpc":"2.0","id":2,"method":"' + method + '","params":{"textDocument":{"uri":"file:///a.boo"},"position":{"line":' + line + ',"character":' + character + '}}}'

	[Test]
	def AdvertisesBothCapabilities():
		Serve()
		result = ReplyTo(1L)["result"] as Dictionary[of string, object]
		capabilities = result["capabilities"] as Dictionary[of string, object]
		assert capabilities["hoverProvider"] == true
		assert capabilities["definitionProvider"] == true

	[Test]
	def HoversOverAMethodCall():
		# "print g.Hello('x')", Hello starts at character 8.
		Serve(Opened, Asking("textDocument/hover", 5, 8))
		hover = ReplyTo(2L)["result"] as Dictionary[of string, object]
		contents = hover["contents"] as Dictionary[of string, object]
		assert contents["kind"] == "markdown"
		assert "def Hello(who as string) as string" in cast(string, contents["value"])

	[Test]
	def HoversOverALocal():
		# "print g.Hello('x')", g is at character 6.
		Serve(Opened, Asking("textDocument/hover", 5, 6))
		contents = (ReplyTo(2L)["result"] as Dictionary[of string, object])["contents"] as Dictionary[of string, object]
		assert "g as Greeter" in cast(string, contents["value"])

	[Test]
	def AnswersNothingWhereThereIsNothing():
		Serve(Opened, Asking("textDocument/hover", 3, 0))
		assert ReplyTo(2L)["result"] is null

	[Test]
	def GoesToTheDefinitionOfAMethod():
		Serve(Opened, Asking("textDocument/definition", 5, 8))
		location = ReplyTo(2L)["result"] as Dictionary[of string, object]
		assert location["uri"] == "file:///a.boo"
		span = location["range"] as Dictionary[of string, object]
		start = span["start"] as Dictionary[of string, object]
		assert start["line"] == 1L

	[Test]
	def GoesToTheDefinitionOfALocal():
		Serve(Opened, Asking("textDocument/definition", 5, 6))
		span = (ReplyTo(2L)["result"] as Dictionary[of string, object])["range"] as Dictionary[of string, object]
		start = span["start"] as Dictionary[of string, object]
		assert start["line"] == 4L

	[Test]
	def AnswersNothingForADocumentThatIsNotOpen():
		asking = '{"jsonrpc":"2.0","id":2,"method":"textDocument/hover","params":{"textDocument":{"uri":"file:///gone.boo"},"position":{"line":0,"character":0}}}'
		Serve(asking)
		assert ReplyTo(2L)["result"] is null
