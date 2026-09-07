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
class SemanticTokensTestFixture:

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
			parsed = JsonCodec.Parse(message) as JsonObject
			continue unless parsed.ContainsKey("id")
			return parsed if Fields.Number(parsed, "id", 0) == id
		return null

	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"class Greeter:\\n\\tdef Hello(who as string) as string:\\n\\t\\treturn who\\n\\ng = Greeter()\\nprint g.Hello(\'x\')\\n"}}}'
	private static final Asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/semanticTokens/full","params":{"textDocument":{"uri":"file:///a.boo"}}}'

	[Test]
	def AdvertisesTheCapability():
		Serve()
		result = ReplyTo(1L)["result"] as JsonObject
		capabilities = Fields.Map(result, "capabilities")
		provider = Fields.Map(capabilities, "semanticTokensProvider")
		assert Fields.Value[of bool](Fields.Of(provider, "full")) == true
		legend = Fields.Map(provider, "legend")
		types = Fields.Items(legend, "tokenTypes")
		assert Fields.Value[of string](types[2]) == "class"
		assert Fields.Value[of string](types[13]) == "method"

	[Test]
	def AnswersWithEncodedTokens():
		Serve(Opened, Asked)
		result = ReplyTo(2L)["result"] as JsonObject
		data = Fields.Items(result, "data")
		assert data.Count > 0
		assert data.Count % 5 == 0
		assert Contains(data, 0, 6, 7, 2)  # Greeter class declaration
		assert Contains(data, 1, 5, 5, 13) # Hello method declaration
		assert Contains(data, 4, 0, 1, 8)  # g local declaration

	[Test]
	def AnswersEmptyForDocumentThatIsNotOpen():
		asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/semanticTokens/full","params":{"textDocument":{"uri":"file:///gone.boo"}}}'
		Serve(asked)
		result = ReplyTo(2L)["result"] as JsonObject
		data = Fields.Items(result, "data")
		assert data.Count == 0

	private static def Contains(data as JsonArray, line as int, character as int, length as int, kind as int) as bool:
		currentLine = 0
		currentCharacter = 0
		i = 0
		while i < data.Count:
			deltaLine = cast(long, data[i])
			deltaStart = cast(long, data[i + 1])
			currentLine += deltaLine
			currentCharacter = (currentCharacter + deltaStart if deltaLine == 0 else deltaStart)
			return true if currentLine == line and currentCharacter == character and cast(long, data[i + 2]) == length and cast(long, data[i + 3]) == kind
			i += 5
		return false
