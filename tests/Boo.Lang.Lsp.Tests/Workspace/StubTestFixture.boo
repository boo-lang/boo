namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.IO
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, TearDownAttribute, Assert)
import Boo.Lang.Parser
import Boo.Lang.Lsp.Workspace

[TestFixture]
class StubTestFixture:
"""A type an assembly owns, written as Boo."""

	analyzer as Analyzer

	[SetUp]
	def Setup():
		analyzer = Analyzer()
		# This fixture is about the Boo rendering, which is no longer the default.
		Decompiler.Language = Decompiler.Boo

	[TearDown]
	def Teardown():
		Decompiler.Language = Decompiler.CSharp

	private def Written(text as string, line as int, character as int) as string:
		document = TextDocument("file:///stub.boo", "boo", 1, text)
		found = Lookup.At(document, analyzer.Bound(document), Position(line, character))
		assert found is not null and found.HasDeclaration, "nothing to point at"
		return File.ReadAllText(Uri(found.DeclarationUri).LocalPath)

	[Test]
	def WritesTheTypeAsBoo():
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		assert written.StartsWith("namespace System.IO\n"), written
		assert "class Path:" in written, written

	[Test]
	def WritesAMemberAsABooSignature():
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		assert "static def GetTempPath() as string:" in written, written

	[Test]
	def LeavesOutWhatTheCompilerNamed():
	"""A property is written as itself, not as the pair behind it."""
		written = Written("import System\n\nprint Console.Out\n", 2, 7)
		assert "def get_Out" not in written, written

	[Test]
	def WritesBooTheParserAccepts():
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		path = Path.Combine(Path.GetTempPath(), "boo-ls-stub-" + Guid.NewGuid().ToString("N") + ".boo")
		File.WriteAllText(path, written)
		try:
			BooParser.ParseFile(path)
		ensure:
			File.Delete(path)

	[Test]
	def TakesTheShapeFromTheDecompilerTree():
	"""What the assembly holds, not only what the type system exposes."""
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		# A field is on the tree and not among the members the type system lists.
		assert "DirectorySeparatorChar as char" in written, written

	[Test]
	def WritesAnArrayTheBooWay():
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		assert "as (char)" in written, written

	[Test]
	def DeclaresWhatAGenericTypeTakes():
		written = Written("import System.Collections.Generic\n\nprint List[of int]()\n", 2, 7)
		assert "class List[of T]:" in written, written.Split(char('\n'))[2]

	[Test]
	def WritesAMethodBodyAsStatements():
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		assert "if path == null:" in written, "no converted body"
		assert "return path.Substring(0, num)" in written, "no converted call"

	[Test]
	def WritesACountedLoopAsAWhile():
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		assert "while num2 >= 0:" in written, "counted loop not converted"

	[Test]
	def NamesWhatItCannotWrite():
	"""What has no Boo spelling is said out loud, not dropped."""
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		assert "# TODO:" in written, "nothing marked unwritten"

	[Test]
	def WritesACompoundAssignment():
		written = Written("import System.IO\n\nprint Path.GetTempPath()\n", 2, 7)
		assert "+=" in written, "compound assignment not converted"

	[Test]
	def WritesASwitchAsAChainOfTests():
		text = "import System\n\nprint Convert.ToString(1, 2)\n"
		written = Written(text, 2, 7)
		assert "elif " in written, "switch not converted"

	[Test]
	def ShowsCSharpWhenThatIsWhatWasAsked():
		Decompiler.Language = Decompiler.CSharp
		try:
			document = TextDocument("file:///stub.boo", "boo", 1, "import System.IO\n\nprint Path.GetTempPath()\n")
			found = Lookup.At(document, analyzer.Bound(document), Position(2, 7))
			assert found.DeclarationUri.EndsWith("System.IO.Path.cs"), found.DeclarationUri
			assert "public static class Path" in File.ReadAllText(Uri(found.DeclarationUri).LocalPath)
		ensure:
			Decompiler.Language = Decompiler.Boo

