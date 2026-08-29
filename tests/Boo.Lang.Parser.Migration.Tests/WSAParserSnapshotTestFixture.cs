namespace Boo.Lang.Parser.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Boo.Lang.Compiler;
using BooCompiler.Tests;

/// <summary>
/// The same characterization suite as ParserSnapshotTestFixture, over the
/// sources under tests/testcases/parser/wsa.
///
/// Those are written without indentation, so only the whitespace agnostic
/// parser can read them and the ordinary fixture leaves them alone.
/// </summary>
[TestFixture]
[Category(ParserSnapshot.Category)]
public class WSAParserSnapshotTestFixture : ParserSnapshotFixture
{
	protected override string SnapshotDirectoryName => "wsa27";

	protected override CompilerPipeline CreatePipeline()
	{
		var pipeline = new Boo.Lang.Compiler.Pipelines.Parse();
		pipeline.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing), new WSABooParsingStep());
		return pipeline;
	}

	[TestCaseSource(typeof(BooCorpus), nameof(BooCorpus.Wsa))]
	public void ParseWsa(string relativePath)
	{
		Check(relativePath);
	}
}

/// <summary>
/// Pins what the ANTLR 4 whitespace agnostic parser produces.
/// </summary>
[TestFixture]
[Category(ParserSnapshot.Category)]
public class WSAParserA4SnapshotTestFixture : WSAParserSnapshotTestFixture
{
	protected override string SnapshotDirectoryName => "wsa4";

	protected override CompilerPipeline CreatePipeline()
	{
		var pipeline = new Boo.Lang.Compiler.Pipelines.Parse();
		pipeline.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing), new Boo.Lang.ParserA4.WSABooParsingStep());
		return pipeline;
	}
}
