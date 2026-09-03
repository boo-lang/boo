namespace BooCompiler.Tests

import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Pipelines

[TestFixture]
partial class DuckyTestFixture(AbstractCompilerTestCase):
	protected override def CustomizeCompilerParameters():
		_parameters.Ducky = true
