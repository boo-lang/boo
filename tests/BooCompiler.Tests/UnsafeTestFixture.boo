namespace BooCompiler.Tests

import NUnit.Framework

[TestFixture]
partial class UnsafeTestFixture(AbstractCompilerTestCase):
	protected override def CustomizeCompilerParameters():
		_parameters.Unsafe = true

	protected override VerifyGeneratedAssemblies as bool:
		get: return false
