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
using BooCompiler.Tests;

/// <summary>
/// The .boo sources the parser fixtures run over.
///
/// Wider than tests/testcases on purpose: src, examples and extras are the
/// largest bodies of real Boo available, and the parser has to read all of them.
/// Paths are root relative with forward slashes, so test case names are the same
/// on every platform.
/// </summary>
public static class BooCorpus
{
	/// <summary>
	/// Sources written without indentation, which only the whitespace agnostic
	/// parser can read.
	/// </summary>
	public const string WsaPath = "tests/testcases/parser/wsa";

	private static readonly string[] SkippedDirectories = { "bin", "obj", ".git", "packages" };

	private static string Root => BooTestCaseUtil.BasePath;

	public static IEnumerable<string> All =>
		Directory
			.EnumerateFiles(Root, "*.boo", SearchOption.AllDirectories)
			.Select(Relative)
			.Where(path => !path.Split('/').Any(segment => SkippedDirectories.Contains(segment)))
			.Where(path => !path.StartsWith(WsaPath + "/", StringComparison.Ordinal))
			.OrderBy(path => path, StringComparer.Ordinal)
			.ToList();

	public static IEnumerable<string> Wsa =>
		Directory
			.EnumerateFiles(Path.Combine(Root, WsaPath.Replace('/', Path.DirectorySeparatorChar)), "*.boo", SearchOption.AllDirectories)
			.Select(Relative)
			.OrderBy(path => path, StringComparer.Ordinal)
			.ToList();

	public static string SourcePathFor(string relativePath) =>
		Path.Combine(Root, relativePath.Replace('/', Path.DirectorySeparatorChar));

	private static string Relative(string path) => path.Substring(Root.Length + 1).Replace('\\', '/');
}
