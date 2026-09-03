namespace Boo.Lang.Parser.Tests

import NUnit.Framework

[TestFixture]
partial class ParserRoundtripTestFixture(AbstractParserTestFixture):
	def RunCompilerTestCase(fname as string):
		RunParserTestCase(fname)
