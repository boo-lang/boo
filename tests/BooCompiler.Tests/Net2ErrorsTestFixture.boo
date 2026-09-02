namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
partial class Net2ErrorsTestFixture(AbstractCompilerErrorsTestFixture):
	override protected def RunCompilerTestCase(name as string):
		Assert.Ignore("Test requires .net 2.") if System.Environment.Version.Major < 2
		super.RunCompilerTestCase(name)
