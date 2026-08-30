#region license
// Copyright (c) the Boo contributors
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Lang.Parser.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using BooCompiler.Tests;

/// <summary>
/// Requires the parser to read every .boo source in the repository, and to
/// survive printing what it read and reading it again.
///
/// The corpus is far wider than the hand written parser fixtures cover. Both
/// properties are statements about this parser alone, with nothing on disk and
/// no second parser to compare against:
///
/// - what is meant to be valid Boo parses, and what is not is rejected
/// - print(parse(s)) equals print(parse(print(parse(s)))), so anything the
///   parser drops or the printer cannot express shows up on the second pass
///
/// They are checked together because the fixed point needs the first parse
/// anyway, and parsing the corpus twice doubled the cost of this fixture.
/// Printed source carries no positions, so the second check is position
/// insensitive and needs no snapshot.
/// </summary>
// Parsing the whole corpus is slow enough to keep out of an ordinary run. Ask
// for it with --filter "TestCategory=Corpus"; CI has a step that does.
[TestFixture]
[Category("Corpus")]
public class ParserCorpusTestFixture
{
	/// <summary>
	/// Sources the parser is supposed to reject. The first mixes tabs and spaces;
	/// the other two are testcases for the errors they raise.
	/// </summary>
	private static readonly HashSet<string> Unparseable = new HashSet<string>
	{
		"tests/testcases/compilation/defaultmember.boo",
		"tests/testcases/errors/single-error-on-missing-import-namespace.boo",
		"tests/testcases/regression/BOO-779-3.boo",
	};

	private static string ExclusionsPath =>
		Path.Combine(BooTestCaseUtil.BasePath,
			"tests", "Boo.Lang.Parser.Tests", "known-print-limitations.txt");

	private static readonly HashSet<string> NoFixedPoint = new HashSet<string>(
		File.Exists(ExclusionsPath)
			? File.ReadAllLines(ExclusionsPath).Where(line => line.Length > 0 && !line.StartsWith("#"))
			: Enumerable.Empty<string>());

	[TestCaseSource(typeof(BooCorpus), nameof(BooCorpus.All))]
	public void ParsesAndReprints(string relativePath)
	{
		// Neither of these reaches the fixed point check, so neither pays the printer.
		if (Unparseable.Contains(relativePath))
		{
			Assert.AreNotEqual(string.Empty, Errors(relativePath, Parser()),
				relativePath + " is listed as unparseable but parsed cleanly");
			return;
		}

		if (NoFixedPoint.Contains(relativePath))
		{
			AssertParses(relativePath, Parser());
			return;
		}

		var first = Print(new FileInput(BooCorpus.SourcePathFor(relativePath)), Parser());
		if (first == null)
			Assert.Fail(relativePath + " did not parse:\n" + Errors(relativePath, Parser()));

		var second = Print(new StringInput(relativePath, first), Parser());
		if (second == null)
			Assert.Fail("reprinting " + relativePath + " produced source the parser rejects:\n" + first);

		Assert.AreEqual(first, second, relativePath + " does not survive a print and reparse");
	}

	[TestCaseSource(typeof(BooCorpus), nameof(BooCorpus.Wsa))]
	public void ParsesWhitespaceAgnostic(string relativePath)
	{
		AssertParses(relativePath, WsaParser());
	}

	private static void AssertParses(string relativePath, ICompilerStep parser)
	{
		var errors = Errors(relativePath, parser);
		if (errors.Length > 0)
			Assert.Fail(relativePath + " did not parse:\n" + errors);
	}

	private static ICompilerStep Parser() => new Boo.Lang.Parser.BooParsingStep();

	private static ICompilerStep WsaParser() => new Boo.Lang.Parser.WSABooParsingStep();

	/// <summary>
	/// The source the printer produces for this input, or null if it does not parse.
	/// </summary>
	private static string Print(ICompilerInput input, ICompilerStep parser)
	{
		var compiler = new BooCompiler();
		var pipeline = new Boo.Lang.Compiler.Pipelines.ParseAndPrint();
		pipeline.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing), parser);
		compiler.Parameters.Pipeline = pipeline;
		compiler.Parameters.OutputWriter = new StringWriter();
		compiler.Parameters.Input.Add(input);

		var context = compiler.Run();
		return context.Errors.Count > 0 ? null : compiler.Parameters.OutputWriter.ToString();
	}

	/// <summary>
	/// Reparses to report why a file failed, on the path where a test is failing anyway.
	/// </summary>
	private static string Errors(string relativePath, ICompilerStep parser)
	{
		var compiler = new BooCompiler();
		var pipeline = new Boo.Lang.Compiler.Pipelines.Parse();
		pipeline.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing), parser);
		compiler.Parameters.Pipeline = pipeline;
		compiler.Parameters.Input.Add(new FileInput(BooCorpus.SourcePathFor(relativePath)));
		return compiler.Run().Errors.ToString(true);
	}
}
