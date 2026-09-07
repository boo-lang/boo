namespace Boo.Lang.Lsp.Tests.Workspace

import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class DocumentStoreTestFixture:

	store as DocumentStore

	[SetUp]
	def Setup():
		store = DocumentStore()

	[Test]
	def KeepsAnOpenedDocument():
		store.Open("file:///a.boo", "boo", 1, "x = 1")
		assert store.Count == 1
		assert store.Get("file:///a.boo").Text == "x = 1"

	[Test]
	def ReplacesTheTextOnChange():
		store.Open("file:///a.boo", "boo", 1, "x = 1")
		store.Change("file:///a.boo", 2, "x = 2")
		document = store.Get("file:///a.boo")
		assert document.Text == "x = 2"
		assert document.Version == 2

	[Test]
	def KeepsTheSameDocumentObjectAcrossAChange():
		store.Open("file:///a.boo", "boo", 1, "x = 1")
		opened = store.Get("file:///a.boo")
		store.Change("file:///a.boo", 2, "x = 2")
		assert store.Get("file:///a.boo") is opened

	[Test]
	def IgnoresAChangeToADocumentThatIsNotOpen():
		store.Change("file:///gone.boo", 2, "x = 2")
		assert store.Count == 0

	[Test]
	def ForgetsAClosedDocument():
		store.Open("file:///a.boo", "boo", 1, "x = 1")
		store.Close("file:///a.boo")
		assert store.Count == 0
		assert store.Get("file:///a.boo") is null

	[Test]
	def ReturnsNullForADocumentItNeverSaw():
		assert store.Get("file:///nowhere.boo") is null
