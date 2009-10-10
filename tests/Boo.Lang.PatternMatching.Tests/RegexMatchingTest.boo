namespace Boo.Lang.PatternMatching.Tests

import System
import NUnit.Framework

import Boo.Lang.PatternMatching

[TestFixture]
class RegexMatchingTest:
	
	[Test]
	def RegexMatch():
		Assert.AreEqual("foo", FooOrBar("aaafoo"))
		Assert.AreEqual("bar", FooOrBar("baabar"))
		
	[Test]
	[ExpectedException(MatchError)]
	def FailedRegexMatch():
		FooOrBar("baz")
	
	[Test]
	def RegexCapture():
		fileName, lineNumber = CaptureFileNameAndLineNumber("foo(32)")
		Assert.AreEqual("foo" , fileName)
		Assert.AreEqual("32", lineNumber)
		
	[Test]
	def TestREValue():
		Assert.AreEqual("one", matchREValue("there is a foo"))
		Assert.AreEqual("one", matchREValue("foo"))
		Assert.AreEqual("four", matchREValue("there is a bar"))
		Assert.AreEqual("three", matchREValue("bar"))
		Assert.AreEqual("qux and number", matchREValue("123 qux"))
		Assert.AreEqual("w4 or d5", matchREValue("quux"))
		Assert.AreEqual("w4 or d5", matchREValue("12345"))
		Assert.AreEqual("fail", matchREValue("qux"))
		
	[Test]
	def TestRECaptureBinding():
		Assert.AreEqual("hello", matchRECapture("hello hello"))
		Assert.AreEqual("3spam", matchRECapture("   spam!"))
		Assert.AreEqual("3", matchRECapture("foofoofoo"))
		Assert.AreEqual("132", matchRECapture("a1b2c3"))
		Assert.AreEqual("no foo!", matchRECapture("bar"))
		Assert.AreEqual("foo 2x!", matchRECapture("foofoobar"))
		Assert.AreEqual("qux and 42", matchRECapture("both 42 and qux"))
		Assert.AreEqual("fail", matchRECapture("hello!"))
		
	[Test]
	def ConstrainedBinding():
		Assert.IsTrue(IsByte("255"))
		Assert.IsFalse(IsByte("256"))
		Assert.IsFalse(IsByte("bb"))
		
	[Test]
	def TestObjectProperty():
		Assert.IsTrue(IsFooItem(Item(Name: "foobar")))
		Assert.IsTrue(IsFooItem(Item(Name: "barfoo")))
		Assert.IsFalse(IsFooItem(Item(Name: "bar")))
		
	def IsFooItem(o):
		match o:
			case Item(Name: /foo/):
				return true
			otherwise:
				return false
			
		
	def IsByte(s as string):
		match s:
			case /(?<value>\d+)/ and int.Parse(value[0].Value) <= byte.MaxValue:
				return true
			otherwise:
				return false
	
	def matchRECapture(value as string):
		match value:
			case /(?<word>\w+)\s+(\k<word>)/:
				return word[0].Value
			case /^(?<whitespace>\s+)(?<word>\w+)!/:
				return whitespace[0].Value.Length.ToString() + word[0].Value
			case /^(f(?<os>oo))+$/:
				return os.Count.ToString()
			case /(\w+(?<number>\d+)){3}/:
				return number[0].Value + number[2].Value + number[1].Value
			case /(?<foo>foo)*bar/:
				if foo.Count > 0:
					return "foo ${foo.Count}x!"
				else:
					return "no foo!"
			case /(?<qux>qux)/ & /(?<number>\d+)/:
				return "${qux[0].Value} and ${number[0].Value}"
			otherwise:
				return "fail"
	
	def matchREValue(value as string):
		match value:
			case /foo/:
				return "one"
			case /^foo$/:
				return "two"
			case /^bar$/:
				return "three"
			case /bar/:
				return "four"
			case /qux/ & /\d+/:
				return "qux and number"
			case /\w{4}/ | /\d{5}/:
				return "w4 or d5"
			otherwise:
				return "fail"
		
	def CaptureFileNameAndLineNumber(value as string):
		match value:
			case result=/(.+)\((\d+)\)/:
				return result.Groups[1].Value, result.Groups[2].Value
		
	def FooOrBar(value as string):
		match value:
			case /foo/:
				return "foo"
			case /bar/:
				return "bar"

