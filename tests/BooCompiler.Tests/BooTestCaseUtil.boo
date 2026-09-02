#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
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

namespace BooCompiler.Tests

import System
import System.IO
import System.Xml.Serialization
import Boo.Lang.Compiler.Ast
import NUnit.Framework

class BooTestCaseUtil:
	"""Helper methods for testing the boo libraries."""

	public static TestCasesPath as string:
		get: return Path.Combine(BasePath, "tests/testcases")

	private static _basePath as string

	public static BasePath as string:
		"""
		The repository root, found by walking up from the test assembly until
		tests/testcases appears. The old build put every test assembly in one
		fixed directory; the SDK gives each project its own.
		"""
		get:
			_basePath = FindBasePath() if _basePath is null
			return _basePath

	private static def FindBasePath() as string:
		dir = DirectoryInfo(AppContext.BaseDirectory)
		while dir is not null:
			return dir.FullName if Directory.Exists(Path.Combine(dir.FullName, "tests", "testcases"))
			dir = dir.Parent

		raise DirectoryNotFoundException(
			"could not locate tests/testcases above " + AppContext.BaseDirectory)

	public static def ParsingStep() as Boo.Lang.Compiler.ICompilerStep:
		return Boo.Lang.Parser.BooParsingStep()

	public static def WsaParsingStep() as Boo.Lang.Compiler.ICompilerStep:
		return Boo.Lang.Parser.WSABooParsingStep()

	public static def ParseFile(fname as string) as Boo.Lang.Compiler.Ast.CompileUnit:
		return Boo.Lang.Parser.BooParser.ParseFile(fname)

	public static def GetTestCasePath(sample as string) as string:
		return Path.Combine(TestCasesPath, sample)

	public static def AssertEqualsByLine(sample as string, expected as string, actual as string):
		eLines = expected.Split(char('\n'))
		aLines = actual.Split(char('\n'))
		lines = Math.Min(eLines.Length, aLines.Length)

		# pula a primeira linha (que contm a declarao
		# <?xml... )
		for i in range(1, lines):
			Assert.AreEqual(eLines[i].Trim(), aLines[i].Trim(), "Line " + (i+1) + " in " + sample)
		#Assertion.AssertEquals("Line count differs for sample " + sample, eLines.Length, aLines.Length);

	public static def AssertEquals(message as string, expected as CompileUnit, actual as CompileUnit):
		AssertEqualsByLine(message, ToXmlString(expected), ToXmlString(actual))

	public static def ToXmlString(node as Node) as string:
		sw = StringWriter()
		XmlSerializer(node.GetType()).Serialize(sw, node)
		return sw.ToString()

	public static def LoadSample(fname as string) as string:
		using sr = File.OpenText(BooTestCaseUtil.GetTestCasePath(fname)):
			return sr.ReadToEnd()
