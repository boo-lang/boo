namespace Boo.Tests.Ast.Compiler
{
	using System;
	using System.IO;
	using Boo.Lang.Ast;
	using Boo.Lang.Compiler;
	using Boo.Lang.Compiler.Pipeline;
	using Boo.Antlr;
	using NUnit.Framework;
	
	[TestFixture]
	public class SemanticsTestCase : AbstractCompilerTestCase
	{
		protected override void SetUpCompilerPipeline(CompilerPipeline pipeline)
		{
			pipeline.
					Add(new Boo.Antlr.BooParsingStep()).
					Add(new UsingResolutionStep()).
					Add(new AstAttributesStep()).
					Add(new AstNormalizationStep()).							
					Add(new SemanticStep()).
					Add(new BooPrinterStep());
		}
		
		protected override string GetTestCasePath(string name)
		{
			return Path.Combine(Path.Combine(_baseTestCasesPath, "../semantics"), name);
		}
		
		[Test]
		public void ModuleMustBecomePrivateFinalClassWithPrivateConstructor()
		{
			RunCompilerTestCase("module0.boo");
		}
		
		[Test]
		public void ClassesMustBePublicByDefault()
		{
			RunCompilerTestCase("classes0.boo");
		}
		
		[Test]
		public void ClassesShouldHaveAutoConstructor()
		{
			RunCompilerTestCase("classes1.boo");
		}
	}
}
