#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Boo.Lang.Compiler.Ast;
using NUnit.Framework;

namespace BooCompiler.Tests
{
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
				Assertion.AssertEquals("Line " + (i+1) + " in " + sample, eLines[i].Trim(), aLines[i].Trim());
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
