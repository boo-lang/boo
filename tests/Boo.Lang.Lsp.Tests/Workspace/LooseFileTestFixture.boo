namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.Collections.Generic
import System.IO
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, TearDownAttribute, Assert)
import Boo.Lang.Lsp.Json
import Boo.Lang.Lsp.Workspace
import System.Text.Json.Nodes

[TestFixture]
class LooseFileTestFixture:
"""
Analysing a document that belongs to no project.

A script beside other scripts is the common shape outside a project, and the
files it imports are the ones next to it. Nothing else beside it is its
business: a directory of unrelated scripts is just as common, and compiling
those together invents errors that are not in the file.
"""

	root as string

	[SetUp]
	def Setup():
		# No .booproj anywhere at or above it: that is what makes it loose.
		root = Path.Combine(Path.GetTempPath(), "boo-ls-" + Guid.NewGuid().ToString("N"))
		Directory.CreateDirectory(root)

	[TearDown]
	def Teardown():
		Directory.Delete(root, true) if Directory.Exists(root)

	private def Messages(diagnostics as JsonArray) as string:
		lines = List[of string]()
		for diagnostic in diagnostics:
			lines.Add(Fields.Text(diagnostic, "message"))
		return string.Join(" | ", lines.ToArray())

	private def Document(text as string) as TextDocument:
		return TextDocument(Uri(Path.Combine(root, "Main.boo")).AbsoluteUri, "boo", 1, text)

	private def WriteSource(name as string, text as string):
		File.WriteAllText(Path.Combine(root, name), text)

	[Test]
	def ResolvesANamespaceASiblingFileDeclares():
		WriteSource("Helpers.boo", "namespace Helpers\n\nclass Greeter:\n\n\tdef Greet() as string:\n\t\treturn \"hi\"\n")
		diagnostics = Analyzer().Bind(Document("import Helpers\n\nprint Greeter().Greet()\n"))
		assert diagnostics.Count == 0, Messages(diagnostics)

	[Test]
	def LeavesOutASiblingNoImportNames():
		WriteSource("Unrelated.boo", "namespace Unrelated\n\nclass Hidden:\n\n\tpass\n")
		diagnostics = Analyzer().Bind(Document("print Hidden()\n"))
		assert diagnostics.Count > 0

	[Test]
	def StillReportsAnImportNoSiblingDeclares():
		WriteSource("Helpers.boo", "namespace Helpers\n\nclass Greeter:\n\n\tpass\n")
		diagnostics = Analyzer().Bind(Document("import Nowhere.At.All\n\nprint 1\n"))
		assert diagnostics.Count > 0

	[Test]
	def ResolvesAMethodOfASiblingModule():
	"""
	Modules of one compilation see each other's top level methods, so a
	script calls the module beside it with no import to name it.
	"""
		WriteSource("git.boo", "def status() as string:\n\treturn \"clean\"\n")
		diagnostics = Analyzer().Bind(Document("print status()\n"))
		assert diagnostics.Count == 0, Messages(diagnostics)

	[Test]
	def NoticesASiblingTurningIntoAScript():
	"""
	What a sibling is gets remembered between binds, so editing one has to
	be enough to change the answer.
	"""
		WriteSource("helper.boo", "def helper() as string:\n\treturn \"hi\"\n")
		assert Analyzer().Bind(Document("print helper()\n")).Count == 0

		# Top level statements make it a program, so it drops out.
		WriteSource("helper.boo", "def helper() as string:\n\treturn \"hi\"\n\nprint helper()\n")
		assert Analyzer().Bind(Document("print helper()\n")).Count > 0

	[Test]
	def LeavesOutASiblingThatIsAScript():
	"""
	Top level statements make a file a program of its own. A directory of
	those is a directory of unrelated scripts, not one program.
	"""
		WriteSource("other.boo", "def helper() as string:\n\treturn \"hi\"\n\nprint helper()\n")
		diagnostics = Analyzer().Bind(Document("print helper()\n"))
		assert diagnostics.Count > 0
