namespace BooCompiler.Tests
{
	using NUnit.Framework;
	using System;
	using Boo.Lang.Compiler;
	
	[TestFixture]
	public class DuckTypingTestFixture : AbstractCompilerTestCase
	{
		protected override CompilerPipeline SetUpCompilerPipeline()
		{
			return new Boo.Lang.Compiler.Pipelines.Quack();
		}
			
		[Test]
		public void BasicDuckTyping()
		{
			RunCompilerTestCase("duck0.boo");
		}
		
		[Test]
		public void PropertyDuckTyping()
		{
			RunCompilerTestCase("duck1.boo");
		}
	}
}
