namespace Boo.Tests.Lang.Compiler
{
	using System;
	using System.IO;
	using NUnit.Framework;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipeline;
	
	public class ErrorPrinterStep : AbstractCompilerComponent, ICompilerStep
	{
		public void Run()
		{
			foreach (CompilerError error in Errors)
			{
				Console.Write(Path.GetFileName(error.LexicalInfo.FileName));
				Console.Write("({0},{1}): ", error.LexicalInfo.Line, error.LexicalInfo.StartColumn);
				Console.Write("{0}: ", error.Code);
				Console.WriteLine(error.Message);
			}
		}
	}
	
	[TestFixture]
	public class CompilerErrorsTestCase : AbstractCompilerTestCase
	{
		[Test]
		public void TestReturnTypes()
		{
			RunCompilerTestCase("return0.boo");
		}
		
		protected override void SetUpCompilerPipeline(CompilerPipeline pipeline)
		{
			pipeline.
					Add(new Boo.Antlr.BooParsingStep()).
					Add(new ImportResolutionStep()).
					Add(new AstAttributesStep()).
					Add(new MacroExpansionStep()).
					Add(new AstNormalizationStep()).							
					Add(new SemanticStep()).
					Add(new ErrorPrinterStep());
		}
		
		protected override bool IgnoreErrors
		{
			get
			{
				return true;
			}
		}
		
		protected override string GetTestCasePath(string name)
		{
			return Path.Combine(Path.Combine(_baseTestCasesPath, "../errors"), name);
		}		
	}
}
