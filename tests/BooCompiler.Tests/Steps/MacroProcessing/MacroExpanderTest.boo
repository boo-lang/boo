namespace BooCompiler.Tests.Steps.MacroProcessing

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Steps.MacroProcessing
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Environments
import NUnit.Framework

class NeverCompilingMacroCompiler(MacroCompiler):
	"""
	Answers for the one macro definition it is told to expect and compiles
	nothing, in place of a strict mock. Being asked about anything else is a
	failure, and the counts say it was asked at all.
	"""

	_expected as TypeDefinition

	Expected as TypeDefinition:
		get: return _expected
		set: _expected = value

	[getter(AskedIfCompiled)]
	_askedIfCompiled = 0

	[getter(AskedToCompile)]
	_askedToCompile = 0

	override def AlreadyCompiled(node as TypeDefinition) as bool:
		assert _expected is node
		++_askedIfCompiled
		return false

	override def Compile(node as TypeDefinition) as Type:
		assert _expected is node
		++_askedToCompile
		return null

[TestFixture]
class MacroExpanderTest:
	[Test]
	def MacroCompilerIsTakenFromTheEnvironment():
		compiler = NeverCompilingMacroCompiler()

		ActiveEnvironment.With(CompilerContextEnvironmentWith(compiler)):
			module = CreateModule()
			macroApplication = MacroStatement(LexicalInfo("file.boo", 1, 1), "foo")
			module.Globals.Add(macroApplication)

			macroDefinition = CreateClassOn(module, "FooMacro")

			compiler.Expected = macroDefinition

			expander = My[of MacroExpander].Instance
			Assert.IsFalse(expander.ExpandAll())

			errors = CompilerErrors()
			Assert.AreEqual(1, errors.Count)
			Assert.AreEqual(CompilerErrorFactory.AstMacroMustBeExternal(macroApplication, macroDefinition.Entity as IType).ToString(), errors[0].ToString())

		Assert.AreEqual(1, compiler.AskedIfCompiled)
		Assert.AreEqual(1, compiler.AskedToCompile)

	private def CompilerErrors() as CompilerErrorCollection:
		return My[of CompilerErrorCollection].Instance

	private def CreateClassOn(module as Module, className as string) as ClassDefinition:
		classDefinition = CodeBuilder().CreateClass(className).ClassDefinition
		module.Members.Add(classDefinition)
		return classDefinition

	private def CreateModule() as Module:
		module = CodeBuilder().CreateModule("test", null)
		CompileUnit().Modules.Add(module)
		return module

	private def CompileUnit() as CompileUnit:
		return My[of CompileUnit].Instance

	private def CodeBuilder() as BooCodeBuilder:
		return My[of BooCodeBuilder].Instance

	private def CompilerContextEnvironmentWith(compiler as MacroCompiler) as IEnvironment:
		parameters = CompilerParameters(false, Environment: ClosedEnvironment(compiler))
		return CompilerContext(parameters).Environment
