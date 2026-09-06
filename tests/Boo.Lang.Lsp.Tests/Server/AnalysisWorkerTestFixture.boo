namespace Boo.Lang.Lsp.Tests.Server

import System.Collections.Generic
import System.Threading
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, TearDownAttribute, Assert)
import Boo.Lang.Lsp.Server
import Boo.Lang.Lsp.Workspace

[TestFixture]
class AnalysisWorkerTestFixture:
"""
The worker compiles off the message loop, so a slow bind cannot stop the
server answering. Tests drive it with a short debounce and wait for idle.
"""

	worker as AnalysisWorker
	analyzed as List[of TextDocument]
	gate as object

	[SetUp]
	def Setup():
		analyzed = List[of TextDocument]()
		gate = object()
		worker = AnalysisWorker(Record, 20)
		worker.Start()

	[TearDown]
	def Teardown():
		worker.Stop()

	private def Record(document as TextDocument):
		lock gate:
			analyzed.Add(document)

	private def Analyzed():
		lock gate:
			return List[of TextDocument](analyzed)

	private def Document(version as int, text as string):
		return TextDocument("file:///a.boo", "boo", version, text)

	[Test]
	def CompilesWhatIsSubmitted():
		worker.Submit(Document(1, "x = 1"))
		assert worker.WaitForIdle(5000)
		assert Analyzed().Count == 1

	[Test]
	def CompilesOnlyTheNewestOfABurst():
		for version in range(1, 11):
			worker.Submit(Document(version, "x = ${version}"))
		assert worker.WaitForIdle(5000)
		done = Analyzed()
		# The burst arrives well inside the debounce, so it coalesces.
		assert done.Count < 10
		assert done[done.Count - 1].Version == 10

	[Test]
	def CompilesAgainAfterAPause():
		worker.Submit(Document(1, "x = 1"))
		assert worker.WaitForIdle(5000)
		worker.Submit(Document(2, "x = 2"))
		assert worker.WaitForIdle(5000)
		assert Analyzed().Count == 2

	[Test]
	def SkipsAWithdrawnDocument():
		worker.Submit(Document(1, "x = 1"))
		worker.Withdraw("file:///a.boo")
		worker.WaitForIdle(5000)
		assert Analyzed().Count == 0

	[Test]
	def KeepsGoingWhenAnAnalysisRaises():
		failing = AnalysisWorker(Throw, 20)
		failing.Start()
		try:
			failing.Submit(Document(1, "x = 1"))
			assert failing.WaitForIdle(5000)
			failing.Submit(Document(2, "x = 2"))
			assert failing.WaitForIdle(5000)
		ensure:
			failing.Stop()

	private def Throw(document as TextDocument):
		raise System.InvalidOperationException("no")

	[Test]
	def StopsCleanly():
		worker.Submit(Document(1, "x = 1"))
		worker.Stop()
		assert not worker.IsRunning
