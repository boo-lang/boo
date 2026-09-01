namespace Boo.Lang.Parser.Tests

class AbstractWSAParserTestFixture(AbstractParserTestFixture):
	override protected def ParsingStep() as Boo.Lang.Compiler.ICompilerStep:
		return BooCompiler.Tests.BooTestCaseUtil.WsaParsingStep()
