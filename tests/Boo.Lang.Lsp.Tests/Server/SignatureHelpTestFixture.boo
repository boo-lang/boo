namespace Boo.Lang.Lsp.Tests.Server

import System.Collections.Generic
import System.IO
import System.Text
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Protocol
import Boo.Lang.Lsp.Server

[TestFixture]
class SignatureHelpTestFixture:

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

	# s = 'hello' / print s.Replace('a',
	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"s = \'hello\'\\nprint s.Replace(\'a\', \\n"}}}'

	private def Asking(line as int, character as int):
		return '{"jsonrpc":"2.0","id":2,"method":"textDocument/signatureHelp","params":{"textDocument":{"uri":"file:///a.boo"},"position":{"line":' + line + ',"character":' + character + '}}}'

	[Test]
	def AdvertisesTheCapability():
		Serve()
		result = ReplyTo(1L)["result"] as Dictionary[of string, object]
		capabilities = result["capabilities"] as Dictionary[of string, object]
		provider = capabilities["signatureHelpProvider"] as Dictionary[of string, object]
		triggers = provider["triggerCharacters"] as List[of object]
		assert "(" in triggers
		assert "," in triggers

	[Test]
	def AnswersWithTheOverloadsAndTheArgument():
		# "print s.Replace('a', ", the cursor sits after the comma and space.
		Serve(Opened, Asking(1, 21))
		result = ReplyTo(2L)["result"] as Dictionary[of string, object]
		assert result is not null
		signatures = result["signatures"] as List[of object]
		assert signatures.Count > 0
		first = signatures[0] as Dictionary[of string, object]
		assert cast(string, first["label"]).StartsWith("def Replace(")
		parameters = first["parameters"] as List[of object]
		assert parameters.Count > 0
		assert result["activeParameter"] == 1

	[Test]
	def AnswersNothingOutsideACall():
		Serve(Opened, Asking(0, 1))
		assert ReplyTo(2L)["result"] is null
