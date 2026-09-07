namespace Boo.Lang.Lsp.Tests.Json

import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class JsonCodecTestFixture:

	private def Obj(*pairs as (object)):
		result = JsonObject()
		for i in range(0, pairs.Length, 2):
			result[cast(string, pairs[i])] = Node(pairs[i + 1])
		return result

	private def Arr(*items as (object)):
		written = JsonArray()
		for item in items:
			written.Add(Node(item))
		return written

	private def Node(value as object) as JsonNode:
	"""JsonValue.Create has an overload per type and none for object."""
		return null if value is null
		node = value as JsonNode
		return node if node is not null
		return JsonValue.Create(cast(string, value)) if value isa string
		return JsonValue.Create(cast(bool, value)) if value isa bool
		return JsonValue.Create(cast(double, value)) if value isa double or value isa single
		return JsonValue.Create(System.Convert.ToInt64(value))

	[Test]
	def ParsesAnObjectIntoADictionary():
		parsed = JsonCodec.Parse('{"jsonrpc":"2.0","id":7}') as JsonObject
		assert parsed is not null
		assert parsed.Count == 2
		assert Fields.Text(parsed, "jsonrpc") == "2.0"
		assert Fields.Number(parsed, "id", 0) == 7

	[Test]
	def ParsesAnArrayIntoAList():
		parsed = JsonCodec.Parse('[1,"two",true,null]') as JsonArray
		assert parsed is not null
		assert parsed.Count == 4
		assert Fields.Value[of long](parsed[0]) == 1
		assert Fields.Value[of string](parsed[1]) == "two"
		assert Fields.Value[of bool](parsed[2]) == true
		assert parsed[3] is null

	[Test]
	def TellsNullFromAbsent():
		parsed = JsonCodec.Parse('{"a":null}') as JsonObject
		assert parsed.ContainsKey("a")
		assert parsed["a"] is null
		assert not parsed.ContainsKey("b")

	[Test]
	def KeepsAnIntegerIdAnInteger():
		parsed = JsonCodec.Parse('{"id":42}') as JsonObject
		assert Fields.Number(parsed, "id", 0) == 42
		assert JsonCodec.Stringify(parsed) == '{"id":42}'

	[Test]
	def KeepsAStringIdAString():
		parsed = JsonCodec.Parse('{"id":"42"}') as JsonObject
		assert Fields.Text(parsed, "id") == "42"
		assert JsonCodec.Stringify(parsed) == '{"id":"42"}'

	[Test]
	def ParsesNestedStructures():
		parsed = JsonCodec.Parse('{"range":{"start":{"line":3,"character":12}}}') as JsonObject
		start = (Fields.Map(parsed, "range"))["start"] as JsonObject
		assert Fields.Number(start, "line", 0) == 3
		assert Fields.Number(start, "character", 0) == 12

	[Test]
	def WritesNestedStructures():
		message = Obj("jsonrpc", "2.0", "id", 7, "result", Obj("items", Arr(1, "two")))
		assert JsonCodec.Stringify(message) == '{"jsonrpc":"2.0","id":7,"result":{"items":[1,"two"]}}'

	[Test]
	def WritesNullRatherThanOmittingIt():
		assert JsonCodec.Stringify(Obj("result", null)) == '{"result":null}'

	[Test]
	def WritesEmptyObjectsAndArrays():
		assert JsonCodec.Stringify(Obj()) == "{}"
		assert JsonCodec.Stringify(Arr()) == "[]"

	[Test]
	def RoundTripsNonAscii():
		text = "café 日本語 " + char.ConvertFromUtf32(0x1F600)
		json = JsonCodec.Stringify(Obj("text", text))
		back = JsonCodec.Parse(json) as JsonObject
		assert Fields.Text(back, "text") == text

	[Test]
	def RoundTripsEscapes():
		text = 'a"b\\c' + "\n\t/"
		json = JsonCodec.Stringify(Obj("s", text))
		back = JsonCodec.Parse(json) as JsonObject
		assert Fields.Text(back, "s") == text

	[Test]
	def RoundTripsFractionalNumbers():
		back = JsonCodec.Parse(JsonCodec.Stringify(Obj("n", 1.5))) as JsonObject
		assert Fields.Value[of double](Fields.Of(back, "n")) == 1.5

	[Test]
	def RoundTripsBooleans():
		back = JsonCodec.Parse(JsonCodec.Stringify(Obj("t", true, "f", false))) as JsonObject
		assert Fields.Value[of bool](Fields.Of(back, "t")) == true
		assert Fields.Value[of bool](Fields.Of(back, "f")) == false

	[Test]
	def WritesWithoutIndentation():
		assert not JsonCodec.Stringify(Obj("a", Obj("b", 1))).Contains("\n")

	[Test]
	def WritesEveryWidthOfWholeNumber():
		assert JsonCodec.Stringify(cast(sbyte, -8)) == "-8"
		assert JsonCodec.Stringify(cast(byte, 8)) == "8"
		assert JsonCodec.Stringify(cast(ushort, 9)) == "9"
		assert JsonCodec.Stringify(cast(uint, 10)) == "10"

	[Test]
	def WritesAnUnsignedLongTooBigForASignedOne():
		assert JsonCodec.Stringify(ulong.MaxValue) == "18446744073709551615"
