namespace Boo.Lang.Lsp.Tests.Server

import System
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class DocumentSymbolTestFixture(ServerFixture):


	private static final Opened = '{"jsonrpc":"2.0","method":"textDocument/didOpen","params":{"textDocument":{"uri":"file:///a.boo","languageId":"boo","version":1,"text":"class Greeter:\\n\\tdef Hello():\\n\\t\\tpass\\n"}}}'
	private static final Asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/documentSymbol","params":{"textDocument":{"uri":"file:///a.boo"}}}'

	[Test]
	def AdvertisesTheCapability():
		Serve()
		result = ReplyTo(1L)["result"] as JsonObject
		capabilities = Fields.Map(result, "capabilities")
		assert Fields.Value[of bool](Fields.Of(capabilities, "documentSymbolProvider")) == true

	[Test]
	def AnswersWithTheOutline():
		Serve(Opened, Asked)
		symbols = ReplyTo(2L)["result"] as JsonArray
		assert symbols.Count == 1
		greeter = symbols[0] as JsonObject
		assert Fields.Text(greeter, "name") == "Greeter"
		children = Fields.Items(greeter, "children")
		assert children.Count == 1

	[Test]
	def AnswersWithAnEmptyListForADocumentThatIsNotOpen():
		asked = '{"jsonrpc":"2.0","id":2,"method":"textDocument/documentSymbol","params":{"textDocument":{"uri":"file:///gone.boo"}}}'
		Serve(asked)
		symbols = ReplyTo(2L)["result"] as JsonArray
		assert symbols.Count == 0
