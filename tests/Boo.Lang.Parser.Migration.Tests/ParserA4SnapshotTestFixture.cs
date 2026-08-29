namespace Boo.Lang.Parser.Tests;

using NUnit.Framework;
using Boo.Lang.Compiler;

/// <summary>
/// Pins what the ANTLR 4 parser produces over the corpus.
///
/// It was snapshotted against the 2.7 parser's output until the two were shown to
/// compile and run every test identically, at which point holding it to 2.7
/// meant reproducing 2.7's mistakes. ParserDivergenceTestFixture counts what
/// the two still disagree about.
/// </summary>
[TestFixture]
[Category(ParserSnapshot.Category)]
public class ParserA4SnapshotTestFixture : ParserSnapshotTestFixture
{
	protected override string SnapshotDirectoryName => "antlr4";

	protected override CompilerPipeline CreatePipeline()
	{
		var pipeline = new Boo.Lang.Compiler.Pipelines.Parse();
		pipeline.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing), new Boo.Lang.ParserA4.BooParsingStep());
		return pipeline;
	}
}
