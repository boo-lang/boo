namespace Boo.AntlrParser.Tests
{
	using System;
	using System.IO;
	using NUnit.Framework;	
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Steps;
	using Boo.AntlrParser;

	public class AbstractParserTestFixture
	{
		Boo.Lang.Compiler.BooCompiler _compiler;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			_compiler = new Boo.Lang.Compiler.BooCompiler();
			_compiler.Parameters.OutputWriter = new StringWriter();
			_compiler.Parameters.Pipeline = new Boo.Lang.Compiler.Pipelines.ParseAndPrint();			
		}
		
		[SetUp]
		public void SetUp()
		{
			_compiler.Parameters.Input.Clear();
			((StringWriter)_compiler.Parameters.OutputWriter).GetStringBuilder().Length = 0;
		}
		
		protected string GetTestCasePath(string fname)
		{
			return Path.Combine(
						BooCompiler.Tests.BooTestCaseUtil.GetTestCasePath("parser"),
						fname);
		}
		
		protected Boo.Lang.Compiler.Ast.Module ParseTestCase(string fname)
		{
			return BooParser.ParseFile(GetTestCasePath(fname)).Modules[0];
		}
		
		protected void RunParserTestCase(string testfile)
		{
			_compiler.Parameters.Input.Add(new FileInput(GetTestCasePath(testfile)));
			Boo.Lang.Compiler.CompilerContext context = _compiler.Run();
			if (context.Errors.Count > 0)
			{
				Assert.Fail(context.Errors.ToString(true));
			}
				
			Assert.AreEqual(1, context.CompileUnit.Modules.Count, "expected a module as output");				
				
			string expected = context.CompileUnit.Modules[0].Documentation;
			if (null == expected)
			{
				Assert.Fail(string.Format("Test case '{0}' does not have a docstring!", testfile));
			}
			
			string output = _compiler.Parameters.OutputWriter.ToString();
			Assert.AreEqual(expected.Trim(), output.ToString().Trim().Replace("\r\n", "\n"), testfile);
		}
	}
}
