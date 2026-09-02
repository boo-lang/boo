namespace BooCompiler.Tests

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class CompilerContextTest:
	[Test]
	def CompileUnitIsProvidedToTheEnvironment():
		compileUnit = CompileUnit()
		ActiveEnvironment.With(CompilerContext(compileUnit).Environment):
			Assert.AreSame(compileUnit, My[of CompileUnit].Instance)
