namespace Boo.Lang.Parser.Tests
{
	using System;
	using System.IO;
	using NUnit.Framework;
	using Boo.Lang.Compiler.IO;
	using Boo.Lang.Compiler.Steps;
	using Boo.Lang.Parser;

	public class AbstractParserTestFixture
	{
		protected Boo.Lang.Compiler.BooCompiler _compiler;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			_compiler = new Boo.Lang.Compiler.BooCompiler();
			_compiler.Parameters.OutputWriter = new StringWriter();
			_compiler.Parameters.Pipeline = CreatePipeline();			
		}
		
		protected virtual Boo.Lang.Compiler.CompilerPipeline CreatePipeline()
		{
			return new Boo.Lang.Compiler.Pipelines.ParseAndPrint();
		}
		
		[SetUp]
		public void SetUp()
		{
			_compiler.Parameters.Input.Clear();
			((StringWriter)_compiler.Parameters.OutputWriter).GetStringBuilder().Length = 0;
		}
		
		protected virtual string GetRelativeTestCasesPath()
		{
			return "parser";
		}
		
		protected string GetTestCasePath(string fname)
		{
			return Path.Combine(
						BooCompiler.Tests.BooTestCaseUtil.GetTestCasePath(GetRelativeTestCasesPath()),
						fname);
		}
		
		protected virtual Boo.Lang.Compiler.Ast.Module ParseTestCase(string fname)
		{
			return BooParser.ParseFile(GetTestCasePath(fname)).Modules[0];
		}
		
		protected virtual Boo.Lang.Compiler.ICompilerInput GetCompilerInput(string testfile)
		{
			return new FileInput(GetTestCasePath(testfile));
		}
		
		protected string GetDocumentation(Boo.Lang.Compiler.Ast.Module module)
		{
			string doc = module.Documentation;
			if (null == module.Documentation)
			{
				Assert.Fail(string.Format("Test case '{0}' does not have a docstring!", module.LexicalInfo.FileName));
			}
			return doc;
		}
		
		protected void RunParserTestCase(string testfile)
		{
			_compiler.Parameters.Input.Add(GetCompilerInput(testfile));
			Boo.Lang.Compiler.CompilerContext context = _compiler.Run();
			if (context.Errors.Count > 0)
			{
				Assert.Fail(context.Errors.ToString(true));
			}
				
			Assert.AreEqual(1, context.CompileUnit.Modules.Count, "expected a module as output");				
				
			string expected = GetDocumentation(context.CompileUnit.Modules[0]);
			
			string output = _compiler.Parameters.OutputWriter.ToString();
			Assert.AreEqual(expected.Trim(), output.ToString().Trim().Replace("\r\n", "\n"), testfile);
		}
	}
}
