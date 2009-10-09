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

