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

namespace Boo.Lang.Parser.Tests

import System.Linq
import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO

[TestFixture]
public class ParsingStepStateTestFixture:
	"""
	The parsing step runs once over every input, so nothing it remembers from
	one file may change what it reports about the next.
	"""
	// The suppression that swallows resync noise keys on line number alone, so a
	// later file whose error sits on an earlier line looks like a repeat.
	private static final ErrorOnLineThree = "a = 1\nb = 2\nc = = 3\n"
	private static final ErrorOnLineTwo = "d = 4\ne = = 5\n"

	[Test]
	public def ReportsAnErrorInEveryInput():
		Assert.IsNotEmpty(Errors("first.boo", ErrorOnLineThree), "the first input alone")
		Assert.IsNotEmpty(Errors("second.boo", ErrorOnLineTwo), "the second input alone")

		both = Parse(
			StringInput("first.boo", ErrorOnLineThree),
			StringInput("second.boo", ErrorOnLineTwo))

		Assert.IsTrue(both.Any({ error as CompilerError | error.LexicalInfo.FileName == "second.boo" }),
			"the second input's error was dropped: " + Describe(both))

	private static def Errors(name as string, code as string) as (CompilerError):
		return Parse(StringInput(name, code))

	private static def Parse(*inputs as (Boo.Lang.Compiler.ICompilerInput)) as (CompilerError):
		compiler = BooCompiler()
		pipeline = Boo.Lang.Compiler.Pipelines.Parse()
		pipeline.Replace(Boo.Lang.Compiler.Steps.Parsing, Boo.Lang.Parser.BooParsingStep())
		compiler.Parameters.Pipeline = pipeline
		for input in inputs:
			compiler.Parameters.Input.Add(input)
		return compiler.Run().Errors.ToArray()

	private static def Describe(errors as (CompilerError)) as string:
		if errors.Length == 0:
			return "no errors at all"
		return string.Join(", ", errors.Select({ e as CompilerError | e.LexicalInfo.FileName + ":" + e.LexicalInfo.Line }))
