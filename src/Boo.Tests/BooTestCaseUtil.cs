using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Boo.Ast.Parsing;
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

		public static void RunXmlTestCase(string sample)
		{
			Boo.Ast.Module module = ParseTestCase(sample);
			AssertEqualsByLine(sample, LoadSample(Path.ChangeExtension(sample, ".xml")), ToXmlString(module));
		}

		public static void AssertEqualsByLine(string sample, string expected, string actual)
		{
			string[] eLines = expected.Split('\n');
			string[] aLines = actual.Split('\n');
			int lines = Math.Min(eLines.Length, aLines.Length);

			// pula a primeira linha (que contém a declaração
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
