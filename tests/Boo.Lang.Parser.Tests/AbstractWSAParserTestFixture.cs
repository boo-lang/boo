using Boo.Lang.Compiler.Steps;

namespace Boo.Lang.Parser.Tests
{
	public class AbstractWSAParserTestFixture : AbstractParserTestFixture
	{	
		override protected Boo.Lang.Compiler.ICompilerStep ParsingStep()
		{
			return BooCompiler.Tests.BooTestCaseUtil.WsaParsingStep();
		}
	}
}
