namespace Boo.Tests.Ast.Compilation
{
	using System;
	using System.IO;
	using Boo.Ast;
	using Boo.Ast.Compilation;
	using Boo.Ast.Compilation.Steps;
	using Boo.Antlr;
	using NUnit.Framework;
	
	[TestFixture]
	public class SemanticsTestCase : AbstractCompilerTestCase
	{
		protected override void SetUpCompilerPipeline(Pipeline pipeline)
		{
			pipeline.
					Add(new Boo.Antlr.BooParsingStep()).
					Add(new UsingResolutionStep()).
					Add(new AstAttributesStep()).
					Add(new ModuleStep()).
					Add(new AstNormalizationStep()).							
					Add(new SemanticStep()).
					Add(new BooPrinterStep());
		}
		
		protected override string GetTestCasePath(string name)
		{
			return Path.Combine(Path.Combine(_baseTestCasesPath, "../semantics"), name);
		}
		
		[Test]
		public void ClassesMustBePublicByDefault()
		{
			RunCompilerTestCase("classes0.boo");
		}
	}
}
