namespace BooCompiler.Tests

import System.Collections.Generic
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import NUnit.Framework

[TestFixture]
class ExtensionsCompilationTest:
	[Test]
	def MacroMacroCompilation():
		parameters = CompilerParameters(false)
		parameters.References.Add(typeof(IEnumerable[of *]).Assembly)

		parameters.Input.Add(BooLangExtensionsSource("Macros/MacroMacro.boo"))
		parameters.Input.Add(BooLangExtensionsSource("Macros/AssertMacro.boo"))

		parameters.Pipeline = Boo.Lang.Compiler.Pipelines.ResolveExpressions()

		compiler = Boo.Lang.Compiler.BooCompiler(parameters)
		results = compiler.Run()
		Assert.AreEqual(0, results.Errors.Count, results.Errors.ToString())

	private def BooLangExtensionsSource(file as string) as FileInput:
		return FileInput(Path.Combine(BooTestCaseUtil.BasePath, "src/Boo.Lang.Extensions/" + file))
