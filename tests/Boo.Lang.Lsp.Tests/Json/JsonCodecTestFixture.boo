namespace Boo.Lang.Lsp.Tests.Json

import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Json

[TestFixture]
class JsonCodecTestFixture:

	private def Obj(*pairs as (object)):
		result = Dictionary[of string, object]()
		for i in range(0, pairs.Length, 2):
			result[cast(string, pairs[i])] = pairs[i + 1]
		return result

	private def Arr(*items as (object)):
		return List[of object](items)

	[Test]
	def ParsesAnObjectIntoADictionary():
		parsed = JsonCodec.Parse('{"jsonrpc":"2.0","id":7}') as Dictionary[of string, object]
		assert parsed is not null
		assert parsed.Count == 2
		assert parsed["jsonrpc"] == "2.0"
		assert parsed["id"] == 7L

	[Test]
	def ParsesAnArrayIntoAList():
		parsed = JsonCodec.Parse('[1,"two",true,null]') as List[of object]
		assert parsed is not null
		assert parsed.Count == 4
		assert parsed[0] == 1L
		assert parsed[1] == "two"
		assert parsed[2] == true
		assert parsed[3] is null

	[Test]
	def TellsNullFromAbsent():
		parsed = JsonCodec.Parse('{"a":null}') as Dictionary[of string, object]
		assert parsed.ContainsKey("a")
		assert parsed["a"] is null
		assert not parsed.ContainsKey("b")

	[Test]
	def KeepsAnIntegerIdAnInteger():
		parsed = JsonCodec.Parse('{"id":42}') as Dictionary[of string, object]
		assert parsed["id"] == 42L
		assert JsonCodec.Stringify(parsed) == '{"id":42}'

	[Test]
	def KeepsAStringIdAString():
		parsed = JsonCodec.Parse('{"id":"42"}') as Dictionary[of string, object]
		assert parsed["id"] == "42"
		assert JsonCodec.Stringify(parsed) == '{"id":"42"}'

	[Test]
	def ParsesNestedStructures():
		parsed = JsonCodec.Parse('{"range":{"start":{"line":3,"character":12}}}') as Dictionary[of string, object]
		start = (parsed["range"] as Dictionary[of string, object])["start"] as Dictionary[of string, object]
		assert start["line"] == 3L
		assert start["character"] == 12L

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
		back = JsonCodec.Parse(json) as Dictionary[of string, object]
		assert back["text"] == text

	[Test]
	def RoundTripsEscapes():
		text = 'a"b\\c' + "\n\t/"
		json = JsonCodec.Stringify(Obj("s", text))
		back = JsonCodec.Parse(json) as Dictionary[of string, object]
		assert back["s"] == text

	[Test]
	def RoundTripsFractionalNumbers():
		back = JsonCodec.Parse(JsonCodec.Stringify(Obj("n", 1.5))) as Dictionary[of string, object]
		assert back["n"] == 1.5

	[Test]
	def RoundTripsBooleans():
		back = JsonCodec.Parse(JsonCodec.Stringify(Obj("t", true, "f", false))) as Dictionary[of string, object]
		assert back["t"] == true
		assert back["f"] == false

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
