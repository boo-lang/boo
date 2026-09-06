namespace Boo.Lang.Lsp.Tests.Workspace

import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class AnalysisQueueTestFixture:
"""
The queue holds one document per URI: while the user types, only the newest
version of a file is worth compiling.
"""

	queue as AnalysisQueue

	[SetUp]
	def Setup():
		queue = AnalysisQueue()

	private def Document(uri as string, version as int, text as string):
		return TextDocument(uri, "boo", version, text)

	[Test]
	def StartsEmpty():
		assert queue.Count == 0
		assert queue.Drain().Count == 0

	[Test]
	def HoldsASubmittedDocument():
		queue.Submit(Document("file:///a.boo", 1, "x = 1"))
		assert queue.Count == 1
		drained = queue.Drain()
		assert drained.Count == 1
		assert drained[0].Version == 1

	[Test]
	def KeepsOnlyTheNewestVersionOfADocument():
		queue.Submit(Document("file:///a.boo", 1, "x = 1"))
		queue.Submit(Document("file:///a.boo", 2, "x = 2"))
		queue.Submit(Document("file:///a.boo", 3, "x = 3"))
		assert queue.Count == 1
		drained = queue.Drain()
		assert drained.Count == 1
		assert drained[0].Version == 3

	[Test]
	def KeepsDocumentsWithDifferentUrisApart():
		queue.Submit(Document("file:///a.boo", 1, "x = 1"))
		queue.Submit(Document("file:///b.boo", 1, "y = 1"))
		assert queue.Drain().Count == 2

	[Test]
	def EmptiesOnDrain():
		queue.Submit(Document("file:///a.boo", 1, "x = 1"))
		queue.Drain()
		assert queue.Count == 0

	[Test]
	def ForgetsAWithdrawnDocument():
		queue.Submit(Document("file:///a.boo", 1, "x = 1"))
		queue.Withdraw("file:///a.boo")
		assert queue.Count == 0

	[Test]
	def DrainsInTheOrderSubmitted():
		queue.Submit(Document("file:///a.boo", 1, "x = 1"))
		queue.Submit(Document("file:///b.boo", 1, "y = 1"))
		queue.Submit(Document("file:///a.boo", 2, "x = 2"))
		drained = queue.Drain()
		assert drained[0].Uri == "file:///a.boo"
		assert drained[0].Version == 2
		assert drained[1].Uri == "file:///b.boo"
