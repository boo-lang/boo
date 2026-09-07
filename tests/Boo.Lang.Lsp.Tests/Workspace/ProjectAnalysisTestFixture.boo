namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.Collections.Generic
import System.IO
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, TearDownAttribute, Assert)
import Boo.Lang.Lsp.Workspace
import Boo.Lang.Lsp.Json
import System.Text.Json.Nodes

[TestFixture]
class ProjectAnalysisTestFixture:
"""Analysing a document against what its project references."""

	root as string
	output as string

	[SetUp]
	def Setup():
		root = Path.Combine(Path.GetTempPath(), "boo-ls-" + Guid.NewGuid().ToString("N"))
		output = Path.Combine(root, "app", "bin", "Debug", "net10.0")
		Directory.CreateDirectory(output)
		# The project's own assembly only marks where the build output is.
		File.WriteAllText(Path.Combine(root, "app", "App.booproj"), "<Project />")
		File.WriteAllText(Path.Combine(output, "App.dll"), "")

	[TearDown]
	def Teardown():
		Directory.Delete(root, true) if Directory.Exists(root)

	private def ReferenceAFixtureAssembly():
	"""
	Put an assembly in the build output that the test host has not loaded.

	Anything compiled in process would not do: the type system provider is
	shared, so the types would be found whether the reference reached the
	compiler or not.
	"""
		name = "testcentric.engine.metadata.dll"
		File.Copy(Path.Combine(AppContext.BaseDirectory, name), Path.Combine(output, name))

	private def Messages(diagnostics as JsonArray) as string:
		lines = List[of string]()
		for diagnostic in diagnostics:
			lines.Add(Fields.Text(diagnostic, "message"))
		return string.Join(" | ", lines.ToArray())

	private def Document(text as string) as TextDocument:
		return TextDocument(Uri(Path.Combine(root, "app", "Main.boo")).AbsoluteUri, "boo", 1, text)

	[Test]
	def ResolvesATypeFromAnAssemblyTheProjectReferences():
		ReferenceAFixtureAssembly()
		diagnostics = Analyzer().Bind(Document("import TestCentric.Metadata\n\ndef describe(token as MetadataToken) as string:\n\treturn token.ToString()\n"))
		assert diagnostics.Count == 0, Messages(diagnostics)

	[Test]
	def StillReportsATypeNoReferenceProvides():
		ReferenceAFixtureAssembly()
		diagnostics = Analyzer().Bind(Document("import Nowhere.At.All\n\nprint 1\n"))
		assert diagnostics.Count > 0

	private def WriteSource(name as string, text as string):
		File.WriteAllText(Path.Combine(root, "app", name), text)

	[Test]
	def ResolvesATypeDeclaredInAnotherFileOfTheProject():
		WriteSource("Helper.boo", "class Sibling:\n\n\tdef Greet() as string:\n\t\treturn \"hi\"\n")
		diagnostics = Analyzer().Bind(Document("print Sibling().Greet()\n"))
		assert diagnostics.Count == 0, Messages(diagnostics)

	[Test]
	def PointsAtADeclarationInAnotherFileByUri():
		WriteSource("Helper.boo", "class Sibling:\n\n\tdef Greet() as string:\n\t\treturn \"hi\"\n")
		document = Document("print Sibling().Greet()\n")
		found = Lookup.At(document, Analyzer().Bound(document), Position(0, 6))
		assert found.HasDeclaration
		assert found.DeclarationUri == Uri(Path.Combine(root, "app", "Helper.boo")).AbsoluteUri

	private def Labels(items as JsonArray) as string:
		labels = List[of string]()
		for item as JsonObject in items:
			labels.Add(Fields.Text(item, "label"))
		return string.Join(",", labels.ToArray())

	[Test]
	def SuggestsAMemberOfATypeDeclaredInAnotherFileOfTheProject():
		WriteSource("Helper.boo", "class Sibling:\n\n\tdef Greet() as string:\n\t\treturn \"hi\"\n")
		# "print Sibling().", the cursor sits after the dot at character 16.
		items = Completion().At(Document("print Sibling().\n"), Position(0, 16))
		assert "Greet" in Labels(items), Labels(items)

	[Test]
	def CompilesTheOpenDocumentOnlyOnce():
		WriteSource("Main.boo", "class OnlyOnce:\n\tpass\n")
		diagnostics = Analyzer().Bind(Document("class OnlyOnce:\n\tpass\n"))
		assert diagnostics.Count == 0, Messages(diagnostics)

	[Test]
	def ReportsNothingForAFaultInAnotherFile():
		WriteSource("Helper.boo", "print NotDefinedAnywhere\n")
		diagnostics = Analyzer().Bind(Document("print 1\n"))
		assert diagnostics.Count == 0, Messages(diagnostics)

	[Test]
	def FindsAReferenceInAnotherFileOfTheProject():
		WriteSource("Helper.boo", "class Sibling:\n\n\tdef Greet() as string:\n\t\treturn \"hi\"\n")
		document = Document("print Sibling().Greet()\n")
		spans = Lookup.References(document, Analyzer().Bound(document), Position(0, 7))
		uris = List[of string]()
		for span in spans:
			uris.Add(span.Uri)
		assert spans.Count >= 2, "found ${spans.Count}"
		assert uris.Contains(Uri(Path.Combine(root, "app", "Helper.boo")).AbsoluteUri), string.Join(",", uris.ToArray())

	[Test]
	def SaysNothingAboutANameWrittenInAnotherFile():
	"""
	Every file of the project is compiled together, and a name in one of them
	sits at a line and column that means nothing in this one. Prose is where
	it shows: a one letter name lands on a letter of an ordinary word.
	"""
		# The e of this helper is at line 2, column 5.
		WriteSource("Helper.boo", "def g():\n    e = 1\n    print e\n")
		# The e of Writes is at line 2, column 5 of this one.
		document = Document('"""\nWrites a thing.\n"""\nprint 1\n')
		found = Lookup.At(document, Analyzer().Bound(document), Position(1, 4))
		assert found is null, "found ${found.Name} : ${found.Signature}"
