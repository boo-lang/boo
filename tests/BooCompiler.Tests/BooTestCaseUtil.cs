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
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Xml;
	using System.Xml.Serialization;
	using Boo.Lang.Compiler.Ast;
	using NUnit.Framework;

	/// <summary>
	/// Helper methods for testing the boo libraries.
	/// </summary>
	public class BooTestCaseUtil
	{
		public static string TestCasesPath
		{
			get
			{
				Uri codebase = new Uri(Assembly.GetExecutingAssembly().CodeBase);
				Uri path = new Uri(codebase, "../testcases");
				return path.LocalPath;
			}
		}

		public static string GetTestCasePath(string sample)
		{
			return Path.Combine(TestCasesPath, sample);
		}
		
		public static void AssertEqualsByLine(string sample, string expected, string actual)
		{
			string[] eLines = expected.Split('\n');
			string[] aLines = actual.Split('\n');
			int lines = Math.Min(eLines.Length, aLines.Length);

			// pula a primeira linha (que contm a declarao
			// <?xml... )
			for (int i=1; i<lines; ++i)
			{
				Assert.AreEqual(eLines[i].Trim(), aLines[i].Trim(), "Line " + (i+1) + " in " + sample);
			}
			//Assertion.AssertEquals("Line count differs for sample " + sample, eLines.Length, aLines.Length);
		}

		public static void AssertEquals(string message, CompileUnit expected, CompileUnit actual)
		{
			AssertEqualsByLine(message, ToXmlString(expected), ToXmlString(actual));
		}

		public static string ToXmlString(Node node)
		{
			StringWriter sw = new StringWriter();
			new XmlSerializer(node.GetType()).Serialize(sw, node);
			return sw.ToString();
		}

		public static string LoadSample(string fname)
		{
			using (StreamReader sr = File.OpenText(BooTestCaseUtil.GetTestCasePath(fname)))
			{
				return sr.ReadToEnd();
			}
		}
	}
}
