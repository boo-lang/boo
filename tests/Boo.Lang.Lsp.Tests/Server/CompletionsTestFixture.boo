namespace Boo.Lang.Lsp.Tests.Server

import System
import System.Collections.Generic
import System.IO
import System.Text
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Server
import System.Text.Json.Nodes

[TestFixture]
class CompletionsTestFixture:

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

	private def ReplyTo(id as long) as JsonObject:
		stream = MessageStream(MemoryStream(_output.ToArray()), MemoryStream())
		while true:
			message = stream.Read()
			break if message is null
			parsed = JsonCodec.Parse(message) as JsonObject
			continue unless parsed.ContainsKey("id")
			return parsed if Fields.Number(parsed, "id", 0) == id
		return null

	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"s = \'hello\'\\nprint s.\\n"}}}'

	[Test]
	def AdvertisesTheCapability():
		Serve()
		result = ReplyTo(1L)["result"] as JsonObject
		capabilities = Fields.Map(result, "capabilities")
		provider = Fields.Map(capabilities, "completionProvider")
		triggers = Fields.Items(provider, "triggerCharacters")
		assert Fields.Value[of string](triggers[0]) == "."

	[Test]
	def SuggestsMembersAfterADot():
		# "print s." with the cursor at the end of the line.
		asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/completion","params":{"textDocument":{"uri":"file:///a.boo"},"position":{"line":1,"character":8}}}'
		Serve(Opened, asked)
		items = ReplyTo(2L)["result"] as JsonArray
		labels = List[of string]()
		for item as JsonObject in items:
			labels.Add(Fields.Text(item, "label"))
		assert "ToUpper" in labels

	[Test]
	def SuggestsNothingForADocumentThatIsNotOpen():
		asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/completion","params":{"textDocument":{"uri":"file:///gone.boo"},"position":{"line":0,"character":0}}}'
		Serve(asked)
		items = ReplyTo(2L)["result"] as JsonArray
		assert items.Count == 0
