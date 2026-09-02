namespace Boo.Lang.Parser.Tests

import NUnit.Framework

[TestFixture]
partial class WSAParserRoundtripTestFixture(AbstractWSAParserTestFixture):
	def RunCompilerTestCase(fname as string):
		RunParserTestCase(fname)
