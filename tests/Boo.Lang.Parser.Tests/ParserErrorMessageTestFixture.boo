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

import System
import System.Collections.Generic
import System.IO
import System.Text.RegularExpressions
import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import BooCompiler.Tests

[TestFixture]
class ParserErrorMessageTestFixture:
	"""
	Requires malformed Boo to produce the message a person can act on.

	The snippets in scripts/error-patterns.boo each state a message and the code
	that should produce it, and are the source GeneratedErrorPatterns was
	generated from. Nothing ran them: that script is a generator, not a test, so
	the whole mechanism had one incidental assertion in LocalizationTest and
	nothing else.

	The stated message is the specification. Where the parser does not meet it,
	KnownDeviations records what it says instead, so the gap is written down
	rather than asserted away.
	"""
	// Snippets whose message the parser does not produce.
	private static final KnownDeviations = Dictionary[of string, string]()

	static Examples as IEnumerable[of TestCaseData]:
		get:
			path = Path.Combine(BooTestCaseUtil.BasePath, "scripts", "error-patterns.boo")
			text = File.ReadAllText(path)
			blocks = Regex.Matches(text, "^error \"(?<message>[^\"]+)\", \"\"\"\r?\n(?<body>.*?)\r?\n\"\"\"",
				RegexOptions.Multiline | RegexOptions.Singleline)

			cases = List[of TestCaseData]()
			for block as Match in blocks:
				message = block.Groups["message"].Value
				snippets = Regex.Split(block.Groups["body"].Value, "\r?\n---\r?\n")
				for i in range(0, snippets.Length):
					continue if string.IsNullOrWhiteSpace(snippets[i])

					name = string.Format("{0} [{1}]", message, i)
					cases.Add(TestCaseData(name, message, snippets[i]).SetName("{m} " + name))
			return cases

	[TestCaseSource("Examples")]
	def ProducesTheStatedMessage(name as string, message as string, code as string):
		deviation as string
		expected = (deviation if KnownDeviations.TryGetValue(name, deviation) else message)
		Assert.AreEqual(expected, Diagnostic(code), code)

	private static def Diagnostic(code as string) as string:
		"""
		The first parse error for this snippet, without the full stop the compiler
		appends, so expectations read the same as the strings in the script.
		"""
		compiler = BooCompiler()
		compiler.Parameters.OutputWriter = StringWriter()
		pipeline = Boo.Lang.Compiler.Pipelines.Parse()
		pipeline.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing),
			Boo.Lang.Parser.BooParsingStep())
		compiler.Parameters.Pipeline = pipeline
		compiler.Parameters.Input.Add(StringInput("error-example", code))

		context = compiler.Run()
		if context.Errors.Count == 0:
			return "(no error)"
		return context.Errors[0].Message.TrimEnd(char('.'))
