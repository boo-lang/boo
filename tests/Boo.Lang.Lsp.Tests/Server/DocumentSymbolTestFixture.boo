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
class DocumentSymbolTestFixture:

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

	private def ReplyTo(id as long):
		stream = MessageStream(MemoryStream(_output.ToArray()), MemoryStream())
		while true:
			message = stream.Read()
			break if message is null
			parsed = JsonCodec.Parse(message) as Dictionary[of string, object]
			continue unless parsed.ContainsKey("id")
			return parsed if cast(long, parsed["id"]) == id
		return null

	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"class Greeter:\\n\\tdef Hello():\\n\\t\\tpass\\n"}}}'
	private static final Asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/documentSymbol","params":{"textDocument":{"uri":"file:///a.boo"}}}'

	[Test]
	def AdvertisesTheCapability():
		Serve()
		result = ReplyTo(1L)["result"] as Dictionary[of string, object]
		capabilities = result["capabilities"] as Dictionary[of string, object]
		assert capabilities["documentSymbolProvider"] == true

	[Test]
	def AnswersWithTheOutline():
		Serve(Opened, Asked)
		symbols = ReplyTo(2L)["result"] as List[of object]
		assert symbols.Count == 1
		greeter = symbols[0] as Dictionary[of string, object]
		assert greeter["name"] == "Greeter"
		children = greeter["children"] as List[of object]
		assert children.Count == 1

	[Test]
	def AnswersWithAnEmptyListForADocumentThatIsNotOpen():
		asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/documentSymbol","params":{"textDocument":{"uri":"file:///gone.boo"}}}'
		Serve(asked)
		symbols = ReplyTo(2L)["result"] as List[of object]
		assert symbols.Count == 0
