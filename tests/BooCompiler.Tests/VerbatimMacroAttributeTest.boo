namespace BooCompiler.Tests

import System
import System.IO
import System.Reflection
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

[TestFixture]
class VerbatimMacroAttributeTest:
"""
A macro with a verbatim argument takes text the parser keeps as written: in the
file defining it, in the files after it, and in programs referencing and
importing a library defining it.
"""
	// Prints the verbatim text a macro is given.
	private static final PrintText = "ExpressionStatement(MethodInvocationExpression(ReferenceExpression('print'), StringLiteralExpression(text)))"

	[Test]
	def MacroDefinedInALibrary():
		library = Compile("""
namespace Foo

import Boo.Lang.Compiler.Ast

macro foo(text as verbatim):
	yield ${PrintText}
""")
		assert "it's \"odd\none\ntwo" == Run("import Foo\nfoo it's \"odd\nfoo:\n    one\n    two\n", library)

	[Test]
	def NestedMacroDefinedInALibrary():
		library = Compile("""
namespace Foo

import Boo.Lang.Compiler.Ast

macro foo:
	macro bar(text as verbatim):
		yield ${PrintText}
	yield foo.Body
""")
		assert "it's \"odd" == Run("import Foo\nfoo:\n    bar it's \"odd\n", library)

	[Test]
	def MacroWithOtherArgumentsTakesOnlyABlock():
		library = Compile("""
namespace Foo

import Boo.Lang.Compiler.Ast

macro foo(label as string, text as verbatim):
	yield ExpressionStatement(MethodInvocationExpression(ReferenceExpression('print'), StringLiteralExpression(label + ': ' + text)))
""")
		assert "x: it's" == Run("import Foo\nfoo 'x':\n    it's\n", library)

	[Test]
	def ReaderMacroTypeFromALibrary():
		library = Compile("""
namespace Foo

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Parser

class DollarReader(ReaderMacro):
	override def ReadsLine(line as string):
		return line.Contains("$")

[VerbatimMacro(typeof(DollarReader))]
class FooMacro(AbstractAstMacro):
	override def Expand(macro as MacroStatement) as Statement:
		return ExpressionStatement(MethodInvocationExpression(ReferenceExpression('print'), macro.VerbatimBody))
""")
		assert "\$ ls -l" == Run("import Foo\nfoo \$ ls -l\n", library)

	[Test]
	def MacroTypeNamedMacro():
		library = Compile("""
namespace Foo

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast

[VerbatimMacro]
class Macro(AbstractAstMacro):
	override def Expand(macro as MacroStatement) as Statement:
		return null
""")
		context = Parse("import Foo\nMacro foo bar\n", library)
		Assert.AreEqual(0, context.Errors.Count, context.Errors.ToString(true))
		macro = context.CompileUnit.Modules[0].Globals.Statements[0] as Boo.Lang.Compiler.Ast.MacroStatement
		Assert.AreEqual("foo bar", macro.VerbatimBody.Value)

	[Test]
	def ReaderMacroTypeThatCannotBeCreatedIsAnError():
		AssertDeclarationError("AbstractReader' could not be created", """
abstract class AbstractReader(ReaderMacro):
	pass

[VerbatimMacro(typeof(AbstractReader))]
class FooMacro(AbstractAstMacro):
	override def Expand(macro as MacroStatement) as Statement:
		return null
""")

	[Test]
	def ReaderMacroTypeThatIsNotAReaderMacroIsAnError():
		AssertDeclarationError("NotAReader' is not a", """
class NotAReader:
	pass

[VerbatimMacro(typeof(NotAReader))]
class FooMacro(AbstractAstMacro):
	override def Expand(macro as MacroStatement) as Statement:
		return null
""")

	[Test]
	def MacroDefinedByTwoLibrariesWarns():
		code = """
namespace Foo

macro foo(text as verbatim):
	pass
"""
		context = Parse("print 1\n", Compile(code, "foo1"), Compile(code, "foo2"))
		Assert.AreEqual(0, context.Errors.Count, context.Errors.ToString(true))
		Assert.AreEqual(1, context.Warnings.Count, context.Warnings.ToString())
		StringAssert.Contains("FooMacro", context.Warnings[0].Message)

	[Test]
	def MacroDefinedInTheSameFile():
		assert "it's \"odd" == Run("""
import Boo.Lang.Compiler.Ast

macro foo(text as verbatim):
	yield ${PrintText}

foo it's "odd
""")

	[Test]
	def MacroDefinedInAnEarlierFile():
		compiler = Compiler(CompileToMemory(), """
namespace Foo

import Boo.Lang.Compiler.Ast

macro foo(text as verbatim):
	yield ${PrintText}
""")
		compiler.Parameters.Input.Add(StringInput("later", "import Foo\nfoo it's \"odd\n"))
		assert "it's \"odd" == Run(compiler)

	[Test]
	def MacroUsedBeforeItsDefinitionWarns():
		context = Parse("foo 1\nmacro foo(text as verbatim):\n    pass\n")
		Assert.AreEqual(0, context.Errors.Count, context.Errors.ToString(true))
		Assert.AreEqual(1, context.Warnings.Count)
		StringAssert.Contains("'foo' is used before its definition", context.Warnings[0].Message)
		Assert.AreEqual(2, context.Warnings[0].LexicalInfo.Line)

	[Test]
	def OtherUsesOfTheNameBeforeTheDefinitionAreFine():
		context = Parse("foo = 1\nmacro foo(text as verbatim):\n    pass\n")
		Assert.AreEqual(0, context.Errors.Count + context.Warnings.Count, context.Warnings.ToString())

	[Test]
	def VerbatimArgumentMustBeLast():
		compiler = Compiler(CompileToMemory(), "macro foo(text as verbatim, n as int):\n    pass\n")
		compiler.Parameters.OutputType = CompilerOutputType.Library
		StringAssert.Contains("verbatim argument must be the last argument", compiler.Run().Errors.ToString())

	private def AssertDeclarationError(expected as string, declaration as string):
		library = Compile("""
namespace Foo

import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Parser
""" + declaration)
		context = Parse("import Foo\nprint 1\n", library)
		Assert.AreEqual(1, context.Errors.Count, context.Errors.ToString(true))
		StringAssert.Contains(expected, context.Errors[0].Message)
		Assert.AreEqual(1, context.CompileUnit.Modules.Count)

	private def Parse(code as string, *references as (Assembly)) as CompilerContext:
		compiler = Compiler(CompilerPipeline(), code, *references)
		compiler.Parameters.Pipeline.Add(Boo.Lang.Parser.BooParsingStep())
		return compiler.Run()

	private def Compile(code as string, name as string) as Assembly:
		compiler = Compiler(CompileToMemory(), code)
		compiler.Parameters.OutputType = CompilerOutputType.Library
		compiler.Parameters.OutputAssembly = name + ".dll"
		context = compiler.Run()
		Assert.AreEqual(0, context.Errors.Count, context.Errors.ToString(true))
		return context.GeneratedAssembly

	private def Compile(code as string) as Assembly:
		compiler = Compiler(CompileToMemory(), code)
		compiler.Parameters.OutputType = CompilerOutputType.Library
		context = compiler.Run()
		Assert.AreEqual(0, context.Errors.Count, context.Errors.ToString(true))
		return context.GeneratedAssembly

	private def Run(code as string, *references as (Assembly)) as string:
		return Run(Compiler(CompileToMemory(), code, *references))

	private def Run(compiler as BooCompiler) as string:
		context = compiler.Run()
		Assert.AreEqual(0, context.Errors.Count, context.Errors.ToString(true))
		written = StringWriter()
		before = Console.Out
		Console.SetOut(written)
		try:
			context.GeneratedAssembly.EntryPoint.Invoke(null, (null,))
		ensure:
			Console.SetOut(before)
		return written.ToString().Trim()

	private def Compiler(pipeline as CompilerPipeline, code as string, *references as (Assembly)):
		compiler = BooCompiler()
		compiler.Parameters.Pipeline = pipeline
		compiler.Parameters.Input.Add(StringInput("code", code))
		compiler.Parameters.References.Add(typeof(Boo.Lang.Extensions.PrintMacro).Assembly)
		compiler.Parameters.References.Add(typeof(Boo.Lang.Parser.ReaderMacro).Assembly)
		for reference in references:
			compiler.Parameters.References.Add(reference)
		return compiler
