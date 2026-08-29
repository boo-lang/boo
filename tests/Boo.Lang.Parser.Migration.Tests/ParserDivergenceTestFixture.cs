namespace Boo.Lang.Parser.Tests;

using System.IO;
using System.Linq;
using NUnit.Framework;
using BooCompiler.Tests;

/// <summary>
/// Migration scaffolding: delete with the 2.7 parser.
///
/// Counts the files the two parsers describe differently.
///
/// Both are snapshotted over the same corpus, so the difference between the two
/// sets of snapshots is the difference between the parsers. Every one of them is a
/// place the 2.7 parser reports something the ANTLR 4 parser does not
/// reproduce on purpose, and the compiler suite passes on both, so the number
/// is pinned rather than driven to zero. It failing in either direction means
/// a parser changed.
/// </summary>
[TestFixture]
[Category(ParserSnapshot.Category)]
public class ParserDivergenceTestFixture
{
	private const int Expected = 602;

	[Test]
	public void TheParsersDifferOnAKnownNumberOfFiles()
	{
		// The snapshots are local only, so there is nothing to count in CI.
		if (!Snapshotted("antlr27") || !Snapshotted("antlr4") || !Snapshotted("wsa27") || !Snapshotted("wsa4"))
			Assert.Ignore("no parser snapshots; generate them with BOO_PARSER_SNAPSHOT_REGENERATE=1");

		Assert.AreEqual(Expected, Divergent().Count());
	}

	/// <summary>
	/// An empty directory is not a snapshot: the directories survive a run that
	/// wrote nothing, so counting files is what tells us there is output to compare.
	/// </summary>
	private static bool Snapshotted(string name)
	{
		var path = Path.Combine(BooTestCaseUtil.BasePath, "tests", "parser-snapshots", name);
		return Directory.Exists(path)
			&& Directory.EnumerateFiles(path, "*.txt", SearchOption.AllDirectories).Any();
	}

	private static System.Collections.Generic.IEnumerable<string> Divergent()
	{
		foreach (var pair in new[] { new[] { "antlr27", "antlr4" }, new[] { "wsa27", "wsa4" } })
		{
			var from = Path.Combine(BooTestCaseUtil.BasePath, "tests", "parser-snapshots", pair[0]);
			var to = Path.Combine(BooTestCaseUtil.BasePath, "tests", "parser-snapshots", pair[1]);
			if (!Directory.Exists(from) || !Directory.Exists(to))
				continue;

			foreach (var snapshot in Directory.EnumerateFiles(from, "*.txt", SearchOption.AllDirectories))
			{
				var other = Path.Combine(to, snapshot.Substring(from.Length + 1));
				if (!File.Exists(other) || File.ReadAllText(other) != File.ReadAllText(snapshot))
					yield return snapshot;
			}
		}
	}
}
