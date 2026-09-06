namespace Boo.Lang.Lsp.Tests.Workspace

import System
import System.Collections.Generic
import System.IO
import NUnit.Framework(TestFixtureAttribute, TestAttribute, SetUpAttribute, CategoryAttribute, Assert)
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Lsp.Workspace

// Reading every source in the repository is slow enough to keep out of an
// ordinary run. Ask for it with --filter "TestCategory=Corpus".
[TestFixture]
[Category("Corpus")]
class CorpusTestFixture:
"""
Runs the analyser over every .boo source in the repository.

The hand written fixtures cover snippets a few lines long. This covers what
people actually wrote, which is where the shapes the analyser has never seen
turn up.

The whitespace agnostic sources are left out: they need a parser this server
does not choose, so what they report says nothing about the analyser.
"""

	static final WsaPath = "tests/testcases/parser/wsa"

	static final SkippedDirectories = ("bin", "obj", ".git", ".vs", "packages")

	analyzer as Analyzer

	[SetUp]
	def Setup():
		analyzer = Analyzer()

	private static def Root() as string:
	"""The repository root, walking up from the test assembly."""
		directory = DirectoryInfo(AppContext.BaseDirectory)
		while directory is not null:
			return directory.FullName if Directory.Exists(Path.Combine(directory.FullName, "tests", "testcases"))
			directory = directory.Parent
		raise DirectoryNotFoundException("no tests/testcases above " + AppContext.BaseDirectory)

	private static def Sources() as List[of string]:
		root = Root()
		found = List[of string]()
		wsa = Path.Combine(root, WsaPath.Replace(char('/'), Path.DirectorySeparatorChar))
		for path in Directory.EnumerateFiles(root, "*.boo", SearchOption.AllDirectories):
			continue if path.StartsWith(wsa, StringComparison.Ordinal)
			continue if IsSkipped(root, path)
			found.Add(path)
		found.Sort(StringComparer.Ordinal)
		return found

	private static def IsSkipped(root as string, path as string) as bool:
		relative = path.Substring(root.Length).TrimStart(Path.DirectorySeparatorChar)
		for segment in relative.Split(Path.DirectorySeparatorChar):
			return true if segment in SkippedDirectories
		return false

	private static def Relative(root as string, path as string) as string:
		return path.Substring(root.Length + 1).Replace(char('\\'), char('/'))

	private static def ParserRejects(uri as string, text as string) as bool:
	"""Whether the compiler's own parse of this source reports anything."""
		compiler = BooCompiler()
		compiler.Parameters.Pipeline = Parse(BreakOnErrors: false)
		compiler.Parameters.OutputType = CompilerOutputType.Library
		compiler.Parameters.Input.Add(StringInput(uri, text))
		return compiler.Run().Errors.Count > 0

	private static def Errors(diagnostics as List[of object]) as List[of Dictionary[of string, object]]:
		found = List[of Dictionary[of string, object]]()
		for diagnostic in diagnostics:
			entry = cast(Dictionary[of string, object], diagnostic)
			found.Add(entry) if cast(int, entry["severity"]) == Diagnostic.Error
		return found

	private static def Describe(entry as Dictionary[of string, object]) as string:
		span = cast(Dictionary[of string, object], entry["range"])
		start = cast(Dictionary[of string, object], span["start"])
		return "${entry['code']} at ${start['line']}:${start['character']} ${entry['message']}"

	[Test]
	def FindsTheCorpus():
	"""A fixture that read nothing would pass every other test here."""
		sources = Sources()
		Assert.Greater(sources.Count, 2000, "only ${sources.Count} sources found")
		rejected = 0
		for path in sources:
			text = File.ReadAllText(path)
			rejected++ if ParserRejects(Uri(path).AbsoluteUri, text)
		# Both answers have to occur, or the comparison below proves nothing.
		Assert.Greater(rejected, 0, "no source in the corpus is rejected")
		Assert.Less(rejected, sources.Count, "every source in the corpus is rejected")

	[Test]
	def AgreesWithTheParserOnWhatIsAnError():
	"""
	The parse tier is the compiler's parse, so it may not add errors of its
	own or lose the ones the compiler reports.
	"""
		root = Root()
		disagreed = List[of string]()
		for path in Sources():
			text = File.ReadAllText(path)
			uri = Uri(path).AbsoluteUri
			document = TextDocument(uri, "boo", 1, text)
			reported = Errors(analyzer.Parse(document)).Count > 0
			continue if reported == ParserRejects(uri, text)
			disagreed.Add("${Relative(root, path)}: analyser says ${reported}, parser says ${not reported}")
		Assert.IsEmpty(disagreed, string.Join("\n", disagreed.ToArray()))

	[Test]
	def PutsEveryDiagnosticInsideTheDocument():
	"""
	A range outside the text is one no editor can draw, and the conversion
	from the compiler's one based positions is where that goes wrong.
	"""
		root = Root()
		outside = List[of string]()
		for path in Sources():
			text = File.ReadAllText(path)
			document = TextDocument(Uri(path).AbsoluteUri, "boo", 1, text)
			for entry in Errors(analyzer.Parse(document)):
				span = cast(Dictionary[of string, object], entry["range"])
				start = cast(Dictionary[of string, object], span["start"])
				line = cast(int, start["line"])
				character = cast(int, start["character"])
				if line < 0 or line >= document.LineCount or character < 0 or character > document.LineText(line).Length:
					outside.Add("${Relative(root, path)}: ${Describe(entry)}")
		Assert.IsEmpty(outside, string.Join("\n", outside.ToArray()))

	[Test]
	def BuildsAnOutlineForEverySourceTheParserAccepts():
	"""
	Document symbols and semantic tokens both read the parse tree, so a
	source the parser accepts and the tree does not is a file with no
	editor features at all.
	"""
		root = Root()
		missing = List[of string]()
		for path in Sources():
			text = File.ReadAllText(path)
			uri = Uri(path).AbsoluteUri
			continue if ParserRejects(uri, text)
			document = TextDocument(uri, "boo", 1, text)
			missing.Add(Relative(root, path)) if analyzer.ParseTree(document) is null
		Assert.IsEmpty(missing, string.Join("\n", missing.ToArray()))
