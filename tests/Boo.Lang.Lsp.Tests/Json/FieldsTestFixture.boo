namespace Boo.Lang.Lsp.Tests.Json

import NUnit.Framework(TestFixtureAttribute, TestAttribute)
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class FieldsTestFixture:
"""
Every reader answers with a default rather than raising, because a client may
leave out anything the protocol marks optional and may send the wrong kind for
what it does send.
"""

	private def Message() as JsonObject:
		return JsonCodec.Parse("""
		{
			"method": "textDocument/hover",
			"line": 12,
			"nested": { "uri": "file:///a.boo" },
			"items": ["(", ","],
			"nothing": null
		}
		""") as JsonObject

	[Test]
	def ReadsAFieldOfEachKind():
		message = Message()
		assert "textDocument/hover" == Fields.Text(message, "method")
		assert 12 == Fields.Number(message, "line", -1)
		assert "file:///a.boo" == Fields.Text(Fields.Map(message, "nested"), "uri")
		assert 2 == Fields.Items(message, "items").Count

	[Test]
	def AMissingFieldReadsAsTheDefault():
		message = Message()
		assert Fields.Of(message, "absent") is null
		assert Fields.Map(message, "absent") is null
		assert Fields.Text(message, "absent") is null
		assert -1 == Fields.Number(message, "absent", -1)

	[Test]
	def AMissingArrayReadsAsEmptyRatherThanNull():
	"""Callers iterate the result, so Items is the one reader that never answers null."""
		assert 0 == Fields.Items(Message(), "absent").Count
		assert 0 == Fields.Items(null, "absent").Count

	[Test]
	def AFieldOfTheWrongKindReadsAsAMissingOne():
		message = Message()
		assert Fields.Map(message, "line") is null
		assert 0 == Fields.Items(message, "line").Count
		assert Fields.Text(message, "line") is null
		assert -1 == Fields.Number(message, "method", -1)

	[Test]
	def AnExplicitNullReadsAsMissing():
		message = Message()
		assert Fields.Text(message, "nothing") is null
		assert -1 == Fields.Number(message, "nothing", -1)

	[Test]
	def ReadingFromSomethingThatIsNotAnObjectAnswersTheDefault():
	"""Handlers pass whatever arrived, which need not be an object at all."""
		assert Fields.Of(null, "method") is null
		assert Fields.Of("not an object", "method") is null
		assert Fields.Text(JsonCodec.Parse("[1,2]"), "method") is null
		assert -1 == Fields.Number(null, "line", -1)

	[Test]
	def ValueOfANullNodeIsTheDefault():
		assert Fields.Value[of string](null) is null
		assert 0 == Fields.Value[of int](null)
