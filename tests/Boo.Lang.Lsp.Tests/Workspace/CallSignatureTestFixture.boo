namespace Boo.Lang.Lsp.Tests.Workspace

import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class CallSignatureTestFixture:
"""
The cursor is written as | in these sources and stripped before the request,
so each case reads as the text someone had typed.
"""

	help as CallSignature

	[SetUp]
	def Setup():
		help = CallSignature()

	private def At(text as string) as CallSignature.Result:
		cursor = text.IndexOf(char('|'))
		source = text.Remove(cursor, 1)
		document = TextDocument("file:///a.boo", "boo", 1, source)
		return help.At(document, document.PositionAt(cursor))

	private def Labels(found as CallSignature.Result) as string:
		text = ""
		for overload in found.Overloads:
			text += " | " if text.Length > 0
			text += overload.Label
		return text

	[Test]
	def DescribesTheCallBeingWritten():
		found = At("s = 'hello'\nprint s.Replace(|\n")
		assert found is not null
		assert found.Overloads.Count > 0
		assert "def Replace(oldValue as string, newValue as string) as string" in Labels(found), Labels(found)
		assert found.ActiveParameter == 0

	[Test]
	def CountsTheArgumentTheCursorIsOn():
		found = At("s = 'hello'\nprint s.Replace('a', |\n")
		assert found.ActiveParameter == 1

	[Test]
	def CountsTheArgumentOfACallThatIsAWholeStatement():
		# Nothing follows the comma, and no print takes the result either.
		found = At("s = 'hello'\ns.Replace('a', |\n")
		assert found is not null
		assert found.ActiveParameter == 1

	[Test]
	def IgnoresACommaInsideAnArgument():
		found = At("s = 'hello'\nprint s.Replace('a,b', |\n")
		assert found.ActiveParameter == 1

	[Test]
	def IgnoresACommaInsideANestedCall():
		found = At("s = 'hello'\nprint s.Replace(s.Substring(1, 2), |\n")
		assert found.ActiveParameter == 1

	[Test]
	def FindsACallOpenedOnAnEarlierLine():
		found = At("s = 'hello'\nprint s.Replace(\n\t'a',\n\t|\n")
		assert found is not null
		assert found.ActiveParameter == 1

	[Test]
	def AnswersNothingOutsideACall():
		assert At("s = 'hello'\nprint s|\n") is null

	[Test]
	def AnswersNothingInsideAListLiteral():
		assert At("items = [1, |\n") is null

	[Test]
	def AnswersNothingWhenTheNameIsNotAMethod():
		assert At("s = 'hello'\nprint s.Length(|\n") is null
