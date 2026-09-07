namespace Boo.Lang.Lsp.Tests.Workspace

import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Workspace
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class CompletionTestFixture:
"""
The cursor is written as | in these sources and stripped before the request,
so each case reads as the text someone had typed.
"""

	completion as Completion

	[SetUp]
	def Setup():
		completion = Completion()

	private def Suggest(text as string) as JsonArray:
		cursor = text.IndexOf(char('|'))
		source = text.Remove(cursor, 1)
		document = TextDocument("file:///a.boo", "boo", 1, source)
		return completion.At(document, document.PositionAt(cursor))

	private def Labels(items as JsonArray):
		labels = List[of string]()
		for item as JsonObject in items:
			labels.Add(Fields.Text(item, "label"))
		return labels

	private def Find(items as JsonArray, label as string) as JsonObject:
		for item as JsonObject in items:
			return item if Fields.Text(item, "label") == label
		return null

	[Test]
	def SuggestsTheMembersOfALocal():
		labels = Labels(Suggest("s = 'hello'\nprint s.|\n"))
		assert "ToUpper" in labels
		assert "Length" in labels

	[Test]
	def SuggestsFromWhatHasBeenTypedSoFar():
		labels = Labels(Suggest("s = 'hello'\nprint s.ToUp|\n"))
		assert "ToUpper" in labels

	[Test]
	def SuggestsTheMembersOfAType():
		labels = Labels(Suggest("import System\nprint Console.|\n"))
		assert "WriteLine" in labels

	[Test]
	def LeavesOutTheAccessorsBehindProperties():
		labels = Labels(Suggest("s = 'hello'\nprint s.|\n"))
		assert "get_Length" not in labels
		assert "Length" in labels

	[Test]
	def SuggestsChildNamespacesAfterImport():
		labels = Labels(Suggest("import System.|\n"))
		assert "Collections" in labels
		# A namespace is not a member, so nothing from a type shows up here.
		assert "WriteLine" not in labels

	[Test]
	def SuggestsNothingWhereThereIsNoTarget():
		assert Suggest("x = 1\n|\n").Count > 0

	[Test]
	def SuggestsBareLocals():
		labels = Labels(Suggest("greeting = 'hello'\nprint gre|\n"))
		assert "greeting" in labels

	[Test]
	def SuggestsBareParameters():
		labels = Labels(Suggest("def Say(who as string):\n\tprint wh|\n"))
		assert "who" in labels

	[Test]
	def SuggestsBareTypes():
		labels = Labels(Suggest("class Greeter:\n\tpass\n\nx = Gre|\n"))
		assert "Greeter" in labels

	[Test]
	def SuggestsNothingAfterADotOnSomethingUnknown():
		assert Suggest("print nosuchthing.|\n").Count == 0

	[Test]
	def MarksAMethodAsAMethod():
		item = Find(Suggest("s = 'hello'\nprint s.|\n"), "ToUpper")
		assert Fields.Number(item, "kind", 0) == Completion.Method

	[Test]
	def MarksAPropertyAsAProperty():
		assert Fields.Number(Find(Suggest("s = 'hello'\nprint s.|\n"), "Length"), "kind", 0) == Completion.Property

	[Test]
	def MarksANamespaceAsAModule():
		assert Fields.Number(Find(Suggest("import System.|\n"), "Collections"), "kind", 0) == Completion.Module

	[Test]
	def DescribesWhatItSuggests():
		item = Find(Suggest("s = 'hello'\nprint s.|\n"), "Length")
		assert Fields.Text(item, "detail").Length > 0

	[Test]
	def OffersEachNameOnce():
		labels = Labels(Suggest("s = 'hello'\nprint s.|\n"))
		seen = Dictionary[of string, int]()
		for label in labels:
			assert not seen.ContainsKey(label), "${label} was offered more than once"
			seen[label] = 1

	[Test]
	def SaysHowManyOverloadsAMethodHas():
		item = Find(Suggest("s = 'hello'\nprint s.|\n"), "Compare")
		assert "overloads" in Fields.Text(item, "detail")

	[Test]
	def LeavesOutCompilerGeneratedNames():
		labels = Labels(Suggest("import System\nprint Console.|\n"))
		for label in labels:
			assert not label.StartsWith("<"), "${label} is generated, not written"

	[Test]
	def SurvivesACursorOnTheFirstCharacter():
		assert Suggest("|x = 1\n").Count == 0

	[Test]
	def SuggestsAPrivateMemberOfTheEnclosingType():
		text = "class Greeter:\n\tprivate def Secret():\n\t\tpass\n\tdef Say():\n\t\tself.|\n"
		labels = Labels(Suggest(text))
		assert "Secret" in labels

	[Test]
	def LeavesOutAPrivateMemberOfAnotherType():
		text = "class Greeter:\n\tprivate def Secret():\n\t\tpass\n\nclass Caller:\n\tdef Say(g as Greeter):\n\t\tg.|\n"
		labels = Labels(Suggest(text))
		assert "Secret" not in labels

	[Test]
	def SuggestsAProtectedMemberOfABaseType():
		text = "class Base:\n\tprotected def Shared():\n\t\tpass\n\nclass Derived(Base):\n\tdef Say():\n\t\tself.|\n"
		labels = Labels(Suggest(text))
		assert "Shared" in labels
