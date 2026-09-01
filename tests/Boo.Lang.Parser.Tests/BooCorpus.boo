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
import System.Linq
import BooCompiler.Tests

static class BooCorpus:
	"""
	The .boo sources the parser fixtures run over.

	Wider than tests/testcases on purpose: src, examples and extras are the
	largest bodies of real Boo available, and the parser has to read all of them.
	Paths are root relative with forward slashes, so test case names are the same
	on every platform.
	"""
	// Sources written without indentation, which only the whitespace agnostic
	// parser can read.
	public final WsaPath = "tests/testcases/parser/wsa"

	private final SkippedDirectories = ("bin", "obj", ".git", ".vs", "packages")

	private Root as string:
		get: return BooTestCaseUtil.BasePath

	public All as IEnumerable[of string]:
		get:
			files = Directory.EnumerateFiles(Root, "*.boo", SearchOption.AllDirectories)
			return files.Select({ path as string | Relative(path) }) \
				.Where({ path as string | not path.Split(char('/')).Any({ segment as string | SkippedDirectories.Contains(segment) }) }) \
				.Where({ path as string | not path.StartsWith(WsaPath + "/", StringComparison.Ordinal) }) \
				.OrderBy({ path as string | path }, StringComparer.Ordinal) \
				.ToList()

	public Wsa as IEnumerable[of string]:
		get:
			root = Path.Combine(Root, WsaPath.Replace(char('/'), Path.DirectorySeparatorChar))
			files = Directory.EnumerateFiles(root, "*.boo", SearchOption.AllDirectories)
			return files.Select({ path as string | Relative(path) }) \
				.OrderBy({ path as string | path }, StringComparer.Ordinal) \
				.ToList()

	public def SourcePathFor(relativePath as string) as string:
		return Path.Combine(Root, relativePath.Replace(char('/'), Path.DirectorySeparatorChar))

	private def Relative(path as string) as string:
		return path.Substring(Root.Length + 1).Replace(char('\\'), char('/'))
