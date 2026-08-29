namespace Boo.Lang.Parser.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Parser.Tests.Util;
using BooCompiler.Tests;

/// <summary>
/// Migration scaffolding: delete with the 2.7 parser.
///
/// Parses every .boo file in the repository with the ANTLR 2.7 parser and
/// compares the result against the output of an earlier run.
///
/// This is a characterization suite, not a specification: it snapshots what the
/// parser does today, including the cases where it reports an error. It exists
/// so that a second parser implementation can be measured against the whole
/// corpus rather than the 378 files the hand written parser fixtures cover.
///
/// The snapshots are not in the repository. Generate them with
/// BOO_PARSER_SNAPSHOT_REGENERATE=1 before running the suite, and regenerate it
/// the same way after a deliberate change in parser behaviour.
/// </summary>
public abstract class ParserSnapshotFixture
{
	protected virtual string SnapshotDirectoryName => "antlr27";

	protected virtual CompilerPipeline CreatePipeline() => new Boo.Lang.Compiler.Pipelines.Parse();

	protected void Check(string relativePath)
	{
		var goldenPath = ParserSnapshot.GoldenPathFor(SnapshotDirectoryName, relativePath);

		// Parsing before this point would do the work of the whole corpus only to
		// throw it away, which is most of the cost of a run with nothing snapshotted.
		if (!ParserSnapshot.Regenerating && !File.Exists(goldenPath))
			Assert.Ignore("no snapshot for " + relativePath + "; generate it with BOO_PARSER_SNAPSHOT_REGENERATE=1");

		var actual = Render(relativePath);

		if (ParserSnapshot.Regenerating)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(goldenPath));
			File.WriteAllText(goldenPath, actual);
			return;
		}

		var expected = File.ReadAllText(goldenPath);
		if (expected != actual)
			ParserSnapshot.WriteActual(SnapshotDirectoryName, relativePath, actual);

		BooTestCaseUtil.AssertEqualsByLine(relativePath, expected, actual);
	}

	private string Render(string relativePath)
	{
		var compiler = new BooCompiler();
		compiler.Parameters.Pipeline = CreatePipeline();
		compiler.Parameters.Input.Add(new FileInput(ParserSnapshot.SourcePathFor(relativePath)));

		CompilerContext context;
		try
		{
			context = compiler.Run();
		}
		catch (Exception x)
		{
			return "THREW " + x.GetType().Name + ": " + ParserSnapshot.Scrub(x.Message) + "\n";
		}

		var builder = new StringBuilder();
		foreach (var error in context.Errors)
			builder
				.Append("ERROR ").Append(error.Code)
				.Append(" (").Append(error.LexicalInfo.Line).Append(',').Append(error.LexicalInfo.Column)
				.Append("): ").Append(ParserSnapshot.Scrub(error.Message)).Append('\n');

		foreach (var module in context.CompileUnit.Modules)
			builder.Append(AstDump.Of(module));

		return builder.ToString();
	}
}

/// <summary>
/// Parses every .boo file outside the whitespace agnostic corpus with the
/// ANTLR 2.7 parser.
/// </summary>
[TestFixture]
[Category(ParserSnapshot.Category)]
public class ParserSnapshotTestFixture : ParserSnapshotFixture
{
	[TestCaseSource(typeof(ParserSnapshot), nameof(ParserSnapshot.Corpus))]
	public void Parse(string relativePath)
	{
		Check(relativePath);
	}
}

/// <summary>
/// Corpus discovery and path handling shared by the snapshot fixtures.
/// </summary>
public static class ParserSnapshot
{
	/// <summary>
	/// Comparing against the snapshots costs a corpus parse per fixture, so the
	/// project excludes this category by default. Ask for it with
	/// --filter "TestCategory=Snapshots".
	/// </summary>
	public const string Category = "Snapshots";

	public static bool Regenerating => Environment.GetEnvironmentVariable("BOO_PARSER_SNAPSHOT_REGENERATE") == "1";

	public static IEnumerable<string> Corpus => BooCorpus.All;

	public static string SourcePathFor(string relativePath) => BooCorpus.SourcePathFor(relativePath);

	/// <summary>
	/// Saves what a fixture actually produced, so a failure can be read as a
	/// diff against the snapshot rather than one line at a time.
	/// </summary>
	public static void WriteActual(string snapshotDirectoryName, string relativePath, string actual)
	{
		var path = Path.Combine(
			BooTestCaseUtil.BasePath, "tests", "parser-snapshots", "actual", snapshotDirectoryName,
			relativePath.Replace('/', Path.DirectorySeparatorChar) + ".txt");
		Directory.CreateDirectory(Path.GetDirectoryName(path));
		File.WriteAllText(path, actual);
	}

	public static string GoldenPathFor(string snapshotDirectoryName, string relativePath)
	{
		return Path.Combine(
			BooTestCaseUtil.BasePath, "tests", "parser-snapshots", snapshotDirectoryName,
			relativePath.Replace('/', Path.DirectorySeparatorChar) + ".txt");
	}

	/// <summary>
	/// Removes the checkout location from a message so the output is portable.
	/// </summary>
	public static string Scrub(string text)
	{
		if (string.IsNullOrEmpty(text))
			return text;

		return text
			.Replace(BooTestCaseUtil.BasePath + Path.DirectorySeparatorChar, string.Empty)
			.Replace(BooTestCaseUtil.BasePath, string.Empty)
			.Replace('\\', '/');
	}
}
