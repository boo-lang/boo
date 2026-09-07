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

	private def ReplyTo(id as long) as JsonObject:
		stream = MessageStream(MemoryStream(_output.ToArray()), MemoryStream())
		while true:
			message = stream.Read()
			break if message is null
			parsed = JsonCodec.Parse(message) as JsonObject
			continue unless parsed.ContainsKey("id")
			return parsed if Fields.Number(parsed, "id", 0) == id
		return null

	# class Greeter / def Hello(who as string) / g = Greeter() / print g.Hello('x')
	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"class Greeter:\\n\\tdef Hello(who as string) as string:\\n\\t\\treturn who\\n\\ng = Greeter()\\nprint g.Hello(\'x\')\\n"}}}'

	# The same, with a doc string on Hello.
	private static final Documented = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"class Greeter:\\n\\tdef Hello(who as string) as string:\\n\\t\\"\\"\\"Says hello to who.\\"\\"\\"\\n\\t\\treturn who\\n\\ng = Greeter()\\nprint g.Hello(\'x\')\\n"}}}'

	private def Asking(method as string, line as int, character as int):
		return '{"jsonrpc":"2.0","id":2,"method":"' + method + '","params":{"textDocument":{"uri":"file:///a.boo"},"position":{"line":' + line + ',"character":' + character + '}}}'

	[Test]
	def AdvertisesBothCapabilities():
		Serve()
		result = ReplyTo(1L)["result"] as JsonObject
		capabilities = Fields.Map(result, "capabilities")
		assert Fields.Value[of bool](Fields.Of(capabilities, "hoverProvider")) == true
		assert Fields.Value[of bool](Fields.Of(capabilities, "definitionProvider")) == true

	[Test]
	def HoversOverAMethodCall():
		# "print g.Hello('x')", Hello starts at character 8.
		Serve(Opened, Asking("textDocument/hover", 5, 8))
		hover = ReplyTo(2L)["result"] as JsonObject
		contents = Fields.Map(hover, "contents")
		assert Fields.Text(contents, "kind") == "markdown"
		assert "def Hello(who as string) as string" in Fields.Text(contents, "value")

	[Test]
	def HoversOverALocal():
		# "print g.Hello('x')", g is at character 6.
		Serve(Opened, Asking("textDocument/hover", 5, 6))
		contents = (ReplyTo(2L)["result"] as JsonObject)["contents"] as JsonObject
		assert "g as Greeter" in Fields.Text(contents, "value")

	[Test]
	def HoversWithWhatTheDeclarationDocuments():
		# "print g.Hello('x')" is line 6 once the doc string is in.
		Serve(Documented, Asking("textDocument/hover", 6, 8))
		contents = (ReplyTo(2L)["result"] as JsonObject)["contents"] as JsonObject
		value = Fields.Text(contents, "value")
		assert "def Hello(who as string) as string" in value
		assert "Says hello to who." in value

	[Test]
	def AnswersNothingWhereThereIsNothing():
		Serve(Opened, Asking("textDocument/hover", 3, 0))
		assert ReplyTo(2L)["result"] is null

	[Test]
	def GoesToTheDefinitionOfAMethod():
		Serve(Opened, Asking("textDocument/definition", 5, 8))
		location = ReplyTo(2L)["result"] as JsonObject
		assert Fields.Text(location, "uri") == "file:///a.boo"
		span = Fields.Map(location, "range")
		start = Fields.Map(span, "start")
		assert Fields.Number(start, "line", 0) == 1

	[Test]
	def GoesToTheDefinitionOfALocal():
		Serve(Opened, Asking("textDocument/definition", 5, 6))
		span = (ReplyTo(2L)["result"] as JsonObject)["range"] as JsonObject
		start = Fields.Map(span, "start")
		assert Fields.Number(start, "line", 0) == 4

	[Test]
	def AnswersNothingForADocumentThatIsNotOpen():
		asking = '{"jsonrpc":"2.0","id":2,"method":"textDocument/hover","params":{"textDocument":{"uri":"file:///gone.boo"},"position":{"line":0,"character":0}}}'
		Serve(asking)
		assert ReplyTo(2L)["result"] is null

	[Test]
	def AnswersReferencesWithEveryPlaceTheNameIsUsed():
		# Greeter is declared on line 0 and used on line 4.
		Serve(Opened, Asking("textDocument/references", 4, 5))
		found = ReplyTo(2)["result"] as JsonArray
		assert found.Count == 2, JsonCodec.Stringify(found)

	[Test]
	def AnswersRenameWithAnEditForEveryPlace():
		renaming = '{"jsonrpc":"2.0","id":2,"method":"textDocument/rename","params":{"textDocument":{"uri":"file:///a.boo"},"position":{"line":4,"character":5},"newName":"Welcomer"}}'
		Serve(Opened, renaming)
		reply = ReplyTo(2)
		assert not reply.ContainsKey("error"), JsonCodec.Stringify(reply)
		changes = (Fields.Map(reply, "result"))["changes"] as JsonObject
		edits = Fields.Items(changes, "file:///a.boo")
		assert edits.Count == 2, JsonCodec.Stringify(edits)
		first = edits[0] as JsonObject
		assert Fields.Text(first, "newText") == "Welcomer"

	[Test]
	def RefusesToRenameWhatAnAssemblyOwns():
		opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"import System\\nprint Console.Out\\n"}}}'
		renaming = '{"jsonrpc":"2.0","id":2,"method":"textDocument/rename","params":{"textDocument":{"uri":"file:///a.boo"},"position":{"line":1,"character":7},"newName":"Nope"}}'
		Serve(opened, renaming)
		reply = ReplyTo(2)
		assert not reply.ContainsKey("error"), JsonCodec.Stringify(reply)
		assert reply["result"] is null

	private def Starting(options as string):
	"""Drives a server whose first message is the initialize under test."""
		message = '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"initializationOptions":' + options + '}}'
		wire = StringBuilder()
		wire.Append("Content-Length: ${UTF8Encoding(false).GetByteCount(message)}\r\n\r\n").Append(message)
		output = MemoryStream()
		input = MemoryStream(UTF8Encoding(false).GetBytes(wire.ToString()))
		LanguageServer(MessageStream(input, output), 20).Run()

	[Test]
	def TakesTheDecompilerLanguageFromTheClient():
		try:
			Starting('{"decompiler":"csharp"}')
			Assert.AreEqual(Boo.Lang.Lsp.Workspace.Decompiler.CSharp, Boo.Lang.Lsp.Workspace.Decompiler.Language)
		ensure:
			Boo.Lang.Lsp.Workspace.Decompiler.Language = Boo.Lang.Lsp.Workspace.Decompiler.CSharp

	[Test]
	def IgnoresALanguageItDoesNotOffer():
		Starting('{"decompiler":"klingon"}')
		Assert.AreEqual(Boo.Lang.Lsp.Workspace.Decompiler.CSharp, Boo.Lang.Lsp.Workspace.Decompiler.Language)

	[Test]
	def TakesTheDecompilerLanguageFromAConfigurationChange():
		changed = '{"jsonrpc":"2.0","method":"workspace/didChangeConfiguration","params":{"settings":{"boo":{"decompiler":{"language":"csharp"}}}}}'
		try:
			Serve(changed)
			Assert.AreEqual(Boo.Lang.Lsp.Workspace.Decompiler.CSharp, Boo.Lang.Lsp.Workspace.Decompiler.Language)
		ensure:
			Boo.Lang.Lsp.Workspace.Decompiler.Language = Boo.Lang.Lsp.Workspace.Decompiler.CSharp

	[Test]
	def KeepsTheLanguageWhenAConfigurationChangeSaysNothingAboutIt():
		changed = '{"jsonrpc":"2.0","method":"workspace/didChangeConfiguration","params":{"settings":{"boo":{}}}}'
		Serve(changed)
		Assert.AreEqual(Boo.Lang.Lsp.Workspace.Decompiler.CSharp, Boo.Lang.Lsp.Workspace.Decompiler.Language)
