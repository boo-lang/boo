namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.IO
import System.Collections.Generic
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, TearDownAttribute, Assert)
import Boo.Lang.Lsp.Workspace

[TestFixture]
class ShadowTestFixture:
"""Loading build outputs from copies, so a build can still replace them."""

	root as string

	[SetUp]
	def Setup():
		root = Path.Combine(Path.GetTempPath(), "boo-ls-" + Guid.NewGuid().ToString("N"))
		Directory.CreateDirectory(root)

	[TearDown]
	def Teardown():
		Directory.Delete(root, true) if Directory.Exists(root)

	private def Write(relative as string, text as string) as string:
		path = Path.Combine(root, relative.Replace(char('/'), Path.DirectorySeparatorChar))
		Directory.CreateDirectory(Path.GetDirectoryName(path))
		File.WriteAllText(path, text)
		return path

	[Test]
	def CopiesABuildOutput():
		output = Write("app/bin/Debug/net10.0/App.dll", "one")
		copy = Shadow.For(output)
		assert copy != output
		assert Path.GetFileName(copy) == "App.dll"
		assert File.ReadAllText(copy) == "one"

	[Test]
	def CopiesAnIntermediateOutputToo():
		output = Write("app/obj/Debug/net10.0/App.dll", "one")
		assert Shadow.For(output) != output

	[Test]
	def LeavesTheCopyFreeToBeReplacedAtTheOriginal():
		output = Write("app/bin/Debug/net10.0/App.dll", "one")
		copy = Shadow.For(output)
		using stream = File.Open(copy, FileMode.Open, FileAccess.Read, FileShare.Read):
			File.WriteAllText(output, "two")
		assert File.ReadAllText(output) == "two"

	[Test]
	def GivesTheSameCopyForTheSameFile():
		output = Write("app/bin/Debug/net10.0/App.dll", "one")
		assert Shadow.For(output) == Shadow.For(output)

	[Test]
	def GivesAFreshCopyOnceTheFileHasBeenRebuilt():
		output = Write("app/bin/Debug/net10.0/App.dll", "one")
		first = Shadow.For(output)
		File.WriteAllText(output, "a longer second build")
		second = Shadow.For(output)
		assert second != first
		assert File.ReadAllText(second) == "a longer second build"

	[Test]
	def LeavesEverythingElseWhereItIs():
		package = Write("packages/fake/1.0.0/lib/net10.0/Fake.dll", "one")
		assert Shadow.For(package) == package

	[Test]
	def LoadsInPlaceWhatIsNotThereToCopy():
		gone = Path.Combine(root, "app", "bin", "Gone.dll")
		assert Shadow.For(gone) == gone
