namespace Boo.Lang.Parser.Tests

import System
import System.IO
import NUnit.Framework
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Steps
import Boo.Lang.Parser

public class AbstractParserTestFixture:
	protected _compiler as Boo.Lang.Compiler.BooCompiler

	[OneTimeSetUp]
	def SetUpFixture():
		_compiler = Boo.Lang.Compiler.BooCompiler()
		_compiler.Parameters.OutputWriter = StringWriter()
		_compiler.Parameters.Pipeline = CreatePipeline()

	protected virtual def ParsingStep() as Boo.Lang.Compiler.ICompilerStep:
		return BooCompiler.Tests.BooTestCaseUtil.ParsingStep()

	protected virtual def CreatePipeline() as Boo.Lang.Compiler.CompilerPipeline:
		pipeline = Boo.Lang.Compiler.Pipelines.ParseAndPrint()
		pipeline.Replace(typeof(Parsing), ParsingStep())
		return pipeline

	[SetUp]
	def SetUp():
		_compiler.Parameters.Input.Clear()
		(_compiler.Parameters.OutputWriter cast StringWriter).GetStringBuilder().Length = 0

	protected virtual def GetRelativeTestCasesPath() as string:
		return "parser"

	protected def GetTestCasePath(fname as string) as string:
		return Path.Combine(
			BooCompiler.Tests.BooTestCaseUtil.GetTestCasePath(GetRelativeTestCasesPath()),
			fname)

	protected virtual def ParseTestCase(fname as string) as Boo.Lang.Compiler.Ast.Module:
		return BooCompiler.Tests.BooTestCaseUtil.ParseFile(GetTestCasePath(fname)).Modules[0]

	protected virtual def GetCompilerInput(testfile as string) as Boo.Lang.Compiler.ICompilerInput:
		return FileInput(GetTestCasePath(testfile))

	protected def GetDocumentation(module as Boo.Lang.Compiler.Ast.Module) as string:
		doc = module.Documentation
		if null == module.Documentation:
			Assert.Fail(string.Format("Test case '{0}' does not have a docstring!", module.LexicalInfo.FileName))
		return doc

	protected def RunParserTestCase(testfile as string):
		_compiler.Parameters.Input.Add(GetCompilerInput(testfile))
		context = _compiler.Run()
		if context.Errors.Count > 0:
			Assert.Fail(context.Errors.ToString(true))

		Assert.AreEqual(1, context.CompileUnit.Modules.Count, "expected a module as output")

		expected = GetDocumentation(context.CompileUnit.Modules[0])
		output = _compiler.Parameters.OutputWriter.ToString()

		Assert.AreEqual(Normalize(expected), Normalize(output), testfile)

	protected def Normalize(code as string) as string:
		lines = code.Trim().Split(char('\n'))
		for i in range(0, lines.Length):
			lines[i] = lines[i].TrimEnd()
		return String.Join("\n", lines)
