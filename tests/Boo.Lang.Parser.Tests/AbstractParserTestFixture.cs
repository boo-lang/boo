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
		protected Boo.Lang.Compiler.BooCompiler _compilerV4;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			_compiler = new Boo.Lang.Compiler.BooCompiler();
			_compiler.Parameters.OutputWriter = new StringWriter();
			_compiler.Parameters.Pipeline = CreatePipeline();
			
			_compilerV4 = new Boo.Lang.Compiler.BooCompiler();
			_compilerV4.Parameters.OutputWriter = new StringWriter();
			_compilerV4.Parameters.Pipeline = CreatePipelineV4();
		}
		
		protected virtual Boo.Lang.Compiler.CompilerPipeline CreatePipeline()
		{
			return new Boo.Lang.Compiler.Pipelines.ParseAndPrint();
		}
		
		protected virtual Boo.Lang.Compiler.CompilerPipeline CreatePipelineV4()
		{
			var result = new Boo.Lang.Compiler.Pipelines.ParseAndPrint();
			result.Replace(typeof(Boo.Lang.Compiler.Steps.Parsing), new Boo.Lang.ParserV4.BooParsingStep());
			return result;
		}
		
		[SetUp]
		public void SetUp()
		{
			_compiler.Parameters.Input.Clear();
			((StringWriter)_compiler.Parameters.OutputWriter).GetStringBuilder().Length = 0;
			_compilerV4.Parameters.Input.Clear();
			((StringWriter)_compilerV4.Parameters.OutputWriter).GetStringBuilder().Length = 0;
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
		
		protected virtual Boo.Lang.Compiler.Ast.Module ParseTestCaseV4(string fname)
		{
			return Boo.Lang.ParserV4.BooParser.ParseFile(GetTestCasePath(fname)).Modules[0];
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

			Assert.AreEqual(Normalize(expected), Normalize(output), testfile);
		}

		protected void RunParserTestCaseV4(string testfile)
		{
			_compilerV4.Parameters.Input.Add(GetCompilerInput(testfile));
			Boo.Lang.Compiler.CompilerContext context = _compilerV4.Run();
			if (context.Errors.Count > 0)
			{
				Assert.Fail(context.Errors.ToString(true));
			}
				
			Assert.AreEqual(1, context.CompileUnit.Modules.Count, "expected a module as output");
				
			string expected = GetDocumentation(context.CompileUnit.Modules[0]);
			
			string output = _compilerV4.Parameters.OutputWriter.ToString();

			Assert.AreEqual(Normalize(expected), Normalize(output), testfile);
		}

		protected string Normalize(string code)
		{
			string[] lines = code.Trim().Split('\n');
			for (int i=0; i<lines.Length; i++)
			{
				lines[i] = lines[i].TrimEnd();
			}
			return String.Join("\n", lines);
		}
	}
}
