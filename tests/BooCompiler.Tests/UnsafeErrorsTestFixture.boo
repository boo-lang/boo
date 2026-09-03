namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
partial class UnsafeErrorsTestFixture(AbstractCompilerErrorsTestFixture):
	protected override def CustomizeCompilerParameters():
		_parameters.Unsafe = true
