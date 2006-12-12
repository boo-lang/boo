namespace Boo.Lang.Parser.Tests
{
	using System;
	using Boo.Lang.Compiler;	
	using Boo.Lang.Parser;

	public class AbstractWSAParserTestFixture : AbstractParserTestFixture
	{	
		override protected CompilerPipeline CreatePipeline()
		{
			CompilerPipeline pipeline = base.CreatePipeline();
			pipeline.Replace(typeof(BooParsingStep), new WSABooParsingStep());
			return pipeline;
		}
	}
}
