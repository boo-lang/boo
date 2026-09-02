namespace BooCompiler.Tests

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import NUnit.Framework

[TestFixture]
class CompilerErrorTest:
	[Test]
	def VerboseFlagTrueIsRespectedByNestedCompilerError():
		error = ProduceNestedCompilerError()
		StringAssert.Contains("Bar", error.ToString(true))

	[Test]
	def VerboseFlagFalseIsRespectedByNestedCompilerError():
		error = ProduceNestedCompilerError()
		Assert.IsFalse(error.ToString().Contains("Bar"))

	private def ProduceNestedCompilerError() as CompilerError:
		try:
			ThrowNestedCompilerError()
		except e as CompilerError:
			return e
		Assert.Fail("CompilerError exception expected.")
		return null

	private def ThrowNestedCompilerError():
		WrappingInCompilerError(Foo)

	private def Foo():
		WrappingInCompilerError(Bar)

	private def Bar():
		raise CompilerError("error")

	private def WrappingInCompilerError(action as Action):
		try:
			action()
		except e as Exception:
			raise CompilerError(LexicalInfo.Empty, e)
