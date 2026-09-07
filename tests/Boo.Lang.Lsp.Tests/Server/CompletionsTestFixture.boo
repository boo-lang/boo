namespace Boo.Lang.Lsp.Tests.Server

import System
import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class CompletionsTestFixture(ServerFixture):


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
