#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// As a special exception, if you link this library with other files to
// produce an executable, this library does not by itself cause the
// resulting executable to be covered by the GNU General Public License.
// This exception does not however invalidate any other reasons why the
// executable file might be covered by the GNU General Public License.
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
using Boo.Antlr;
using Boo.Ast;
using NUnit.Framework;

namespace Boo.Tests
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

		public static Boo.Ast.Module ParseTestCase(string sample)
		{			
			using (StreamReader reader = File.OpenText(GetTestCasePath(sample)))
			{
				return BooParser.ParseModule(sample, reader, new ParserErrorHandler(OnError));
			}
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

		static void OnError(antlr.RecognitionException x)
		{
			Assertion.Fail(x.ToString());
		}
	}
}
