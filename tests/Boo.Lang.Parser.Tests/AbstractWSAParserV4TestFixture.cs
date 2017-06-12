using Boo.Lang.Compiler.Steps;

namespace Boo.Lang.Parser.Tests
{
	using System;
	using Boo.Lang.Compiler;	
	using Boo.Lang.Parser;

	public class AbstractWSAParserV4TestFixture : AbstractParserTestFixture
	{	
		override protected CompilerPipeline CreatePipelineV4()
		{
			CompilerPipeline pipeline = base.CreatePipeline();
			pipeline.Replace(typeof(Parsing), new Boo.Lang.ParserV4.WSABooParsingStep());
			return pipeline;
		}
	}
}
