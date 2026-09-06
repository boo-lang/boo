namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.Collections.Generic
import System.Threading
import NUnit.Framework(TestFixtureAttribute, TestAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class ConcurrentCompileTestFixture:
"""
The worker binds while the message loop answers hover and completion, so the
server does compile from more than one thread. The compiler cannot, hence the
gate they all pass through.
"""

	private def Document(text as string):
		return TextDocument("file:///a.boo", "boo", 1, text)

	[Test]
	def BindsFromSeveralThreadsAtOnce():
		failures = List[of string]()
		gate = object()
		threads = List[of Thread]()

		for i in range(4):
			thread = Thread() do:
				analyzer = Analyzer()
				for j in range(4):
					try:
						found = analyzer.Bind(Document("s = 'hello'\nprint s.Length\n"))
						unless found.Count == 0:
							lock gate:
								failures.Add("unexpected diagnostics: ${found.Count}")
					except e as Exception:
						lock gate:
							failures.Add(e.GetType().Name + ": " + e.Message)
			thread.IsBackground = true
			threads.Add(thread)

		for thread in threads:
			thread.Start()
		for thread in threads:
			assert thread.Join(60000), "a compile thread did not finish"

		assert failures.Count == 0, string.Join("; ", failures.ToArray())

	[Test]
	def MixesCompletionAndBindingAcrossThreads():
		failures = List[of string]()
		gate = object()
		threads = List[of Thread]()

		binder = Thread() do:
			analyzer = Analyzer()
			for i in range(4):
				try:
					analyzer.Bind(Document("import System\nprint Console.Out\n"))
				except e as Exception:
					lock gate:
						failures.Add("bind: " + e.Message)

		suggester = Thread() do:
			completion = Completion()
			for i in range(4):
				try:
					document = Document("s = 'hello'\nprint s.\n")
					items = completion.At(document, Position(1, 8))
					unless items.Count > 0:
						lock gate:
							failures.Add("no suggestions")
				except e as Exception:
					lock gate:
						failures.Add("complete: " + e.Message)

		threads.Add(binder)
		threads.Add(suggester)
		for thread in threads:
			thread.IsBackground = true
			thread.Start()
		for thread in threads:
			assert thread.Join(60000), "a thread did not finish"

		assert failures.Count == 0, string.Join("; ", failures.ToArray())
