namespace Boo.Lang.Parser.Tests

import System.IO
import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.Ast.Visitors
import Boo.Lang.Compiler.IO
import Boo.Lang.Environments
import Boo.Lang.Parser

class PromptReader(ReaderMacro):
"""
Reads a line starting with $ as verbatim text, in its block or after its name.
"""
	override def ReadBlockLine(line as string) as LineSyntax:
		return (LineSyntax.Verbatim if line.StartsWith("\$") else LineSyntax.Boo)

	override def ReadsLine(line as string) as bool:
		return line.Contains("\$")

class OneLineReader(ReaderMacro):
"""
Reads what follows its name, and none of the lines in its block.
"""
	override def ReadsLine(line as string) as bool:
		return true

[TestFixture]
class VerbatimMacroTestFixture:
	"""
	A macro registered in ParserSettings.ReaderMacros decides which of its
	source is kept as verbatim text instead of being parsed as Boo.
	"""
	[Test]
	def BodyIsKeptAsSource():
		code = "sql:\n    select `a` from t\n    where name = 'it''s'\n"
		macro = Globals(code)[0] cast MacroStatement
		Assert.AreEqual("sql", macro.Name)
		Assert.AreEqual("select `a` from t\nwhere name = 'it''s'", macro.VerbatimBody.Value)
		Assert.IsTrue(macro.Body.IsEmpty)

	[Test]
	def ArgumentsBeforeTheColon():
		code = "sql db, \"people\", rows[1:]:\n    select 1\n"
		macro = Globals(code)[0] cast MacroStatement
		Assert.AreEqual(3, macro.Arguments.Count)
		Assert.AreEqual("db", macro.Arguments[0].ToString())
		Assert.AreEqual("select 1", macro.VerbatimBody.Value)

	[Test]
	def ArgumentsSpanningLines():
		code = "sql foo(1,\n        2):\n    select 1\nprint 1\n"
		globals = Globals(code)
		Assert.AreEqual(2, globals.Count)
		macro = globals[0] cast MacroStatement
		Assert.AreEqual(1, macro.Arguments.Count)
		Assert.AreEqual("select 1", macro.VerbatimBody.Value)

	[Test]
	def ArgumentsThenPrinted():
		code = "sql db:\n    select 1\n"
		printed = (Globals(code)[0] cast MacroStatement).ToCodeString()
		Assert.AreEqual("sql db:\n\tselect 1\n", printed.Replace("\r\n", "\n"))
		Assert.AreEqual("select 1", (Globals(printed)[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def ClosureAssignedToAListedNameParsesAsBoo():
		code = "sql = def():\n    return 1\n"
		statement = Globals(code)[0] cast ExpressionStatement
		Assert.IsInstanceOf[of BlockExpression]((statement.Expression cast BinaryExpression).Right)

	[Test]
	def InvocationOfAListedNameParsesAsBoo():
		code = "sql(1).foo def():\n    return 1\n"
		Assert.IsInstanceOf[of ExpressionStatement](Globals(code)[0])

	[Test]
	def ColonNotEndingTheLineParsesAsBoo():
		code = "sql x: print 1\n"
		macro = Globals(code)[0] cast MacroStatement
		Assert.IsNull(macro.VerbatimBody)
		Assert.AreEqual(1, macro.Body.Statements.Count)

	[Test]
	def TextBooCannotLexIsAccepted():
		code = "sql:\n    -- it's not a string\n    select \"\n"
		macro = Globals(code)[0] cast MacroStatement
		Assert.AreEqual("-- it's not a string\nselect \"", macro.VerbatimBody.Value)

	[Test]
	def DeeperIndentationStaysInTheBody():
		code = "sql:\n    from (\n        select 1\n    ) x\nprint 1\n"
		globals = Globals(code)
		Assert.AreEqual(2, globals.Count)
		Assert.AreEqual("from (\n    select 1\n) x", (globals[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def BlankLinesInsideAreKeptAndTrailingOnesDropped():
		code = "sql:\n    a\n\n  \n    b\n\n\nprint 1\n"
		globals = Globals(code)
		Assert.AreEqual(2, globals.Count)
		Assert.AreEqual("a\n\n\nb", (globals[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def TabIndentedBody():
		code = "sql:\n\tselect 1\n\t\tfrom t\n"
		Assert.AreEqual("select 1\n\tfrom t", (Globals(code)[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def CrLfLineEndsBecomeLf():
		code = "sql:\r\n    a\r\n    b\r\nprint 1\r\n"
		globals = Globals(code)
		Assert.AreEqual(2, globals.Count)
		Assert.AreEqual("a\nb", (globals[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def InsideAMethod():
		code = "def f():\n    sql:\n        select 1\n    print 1\n"
		method = Parse(code).Members[0] cast Method
		Assert.AreEqual(2, method.Body.Statements.Count)
		Assert.AreEqual("select 1", (method.Body.Statements[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def EnclosingBlockEndsAfterTheBody():
		code = "def f():\n    sql:\n        select 1\n        from t\nprint 1\n"
		end = (Parse(code).Members[0] cast Method).Body.EndSourceLocation
		Assert.AreEqual(4, end.Line)
		Assert.AreEqual(15, end.Column)

	[Test]
	def InsideAClass():
		code = "class C:\n    sql:\n        select 1\n    def f():\n        pass\n"
		type = Parse(code).Members[0] cast ClassDefinition
		Assert.AreEqual(2, type.Members.Count)
		macro = (type.Members[0] cast StatementTypeMember).Statement cast MacroStatement
		Assert.AreEqual("select 1", macro.VerbatimBody.Value)

	[Test]
	def UnlistedNameParsesAsBoo():
		code = "sql:\n    foo bar\n"
		unit = BooParser.ParseReader(ParserSettings(), "code", StringReader(code))
		macro = unit.Modules[0].Globals.Statements[0] cast MacroStatement
		Assert.IsNull(macro.VerbatimBody)
		Assert.AreEqual(1, macro.Body.Statements.Count)

	[Test]
	def OtherUsesOfAListedNameParseAsBoo():
		code = "sql = 3\nsql 1, 2\nprint sql\n"
		globals = Globals(code)
		Assert.AreEqual(3, globals.Count)
		Assert.IsInstanceOf[of ExpressionStatement](globals[0])
		macro = globals[1] cast MacroStatement
		Assert.AreEqual(2, macro.Arguments.Count)
		Assert.IsNull(macro.VerbatimBody)

	[Test]
	def CloneKeepsTheBody():
		macro = Globals("sql:\n    select 1\n")[0] cast MacroStatement
		clone = macro.CloneNode()
		Assert.AreEqual("select 1", clone.VerbatimBody.Value)
		AssertAt(2, 5, clone.VerbatimBody.LexicalInfo)

	[Test]
	def PrinterWritesTheBodyBack():
		code = "sql:\n    select {1}\n\n    from t\n"
		printed = (Globals(code)[0] cast MacroStatement).ToCodeString()
		Assert.AreEqual("sql :\n\tselect {1}\n\n\tfrom t\n", printed.Replace("\r\n", "\n"))
		Assert.AreEqual("select {1}\n\nfrom t", (Globals(printed)[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def WhitespaceAgnosticBodyEndsAtEnd():
		code = "sql:\nselect 'it''s'\n  from t\nend\nprint 1\n"
		globals = ParseWsa(code).Globals.Statements
		Assert.AreEqual(2, globals.Count)
		Assert.AreEqual("select 'it''s'\n  from t", (globals[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def WhitespaceAgnosticInsideAMethod():
		code = "def f():\nsql db:\n    select 1\nend\nprint 1\nend\n"
		method = ParseWsa(code).Members[0] cast Method
		Assert.AreEqual(2, method.Body.Statements.Count)
		macro = method.Body.Statements[0] cast MacroStatement
		Assert.AreEqual(1, macro.Arguments.Count)
		Assert.AreEqual("select 1", macro.VerbatimBody.Value)

	[Test]
	def WhitespaceAgnosticPrinterWritesEnd():
		macro = ParseWsa("sql:\n    select 1\nend\n").Globals.Statements[0] cast MacroStatement
		writer = StringWriter()
		macro.Accept(BooPrinterVisitor(writer, BooPrinterVisitor.PrintOptions.WSA))
		printed = writer.ToString().Replace("\r\n", "\n")
		Assert.AreEqual("sql :\n\tselect 1\nend\n", printed)
		Assert.AreEqual("select 1", (ParseWsa(printed).Globals.Statements[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def CommandLinesMixWithBoo():
		code = "rr n:\n    if n == 1:\n        say it's beep\n        print 2\n    else:\n        evade\n"
		macro = Globals(code)[0] cast MacroStatement
		Assert.IsNull(macro.VerbatimBody)
		test = macro.Body.Statements[0] cast IfStatement
		say = test.TrueBlock.Statements[0] cast MacroStatement
		Assert.AreEqual("say", say.Name)
		Assert.AreEqual("it's beep", say.VerbatimBody.Value)
		Assert.IsTrue(say.IsVerbatimLine)
		Assert.IsNull((test.TrueBlock.Statements[1] cast MacroStatement).VerbatimBody)
		evade = test.FalseBlock.Statements[0] cast MacroStatement
		Assert.IsNull(evade.VerbatimBody)

	[Test]
	def CommandNamesOutsideTheirMacroAreBoo():
		code = "say 1\nrr:\n    say one two\nsay 3\n"
		globals = Globals(code)
		Assert.IsNull((globals[0] cast MacroStatement).VerbatimBody)
		Assert.AreEqual("one two", ((globals[1] cast MacroStatement).Body.Statements[0] cast MacroStatement).VerbatimBody.Value)
		Assert.IsNull((globals[2] cast MacroStatement).VerbatimBody)

	[Test]
	def CommandNamesUsedAsBooInsideTheirMacro():
		code = "rr:\n    say = 1\n    say(2)\n    say.x()\n    say += 3\n"
		for statement in (Globals(code)[0] cast MacroStatement).Body.Statements:
			Assert.IsInstanceOf[of ExpressionStatement](statement)

	[Test]
	def CommandLinePrinted():
		code = "rr:\n    say beep beep\n    evade\n"
		printed = (Globals(code)[0] cast MacroStatement).ToCodeString().Replace("\r\n", "\n")
		Assert.AreEqual("rr :\n\tsay beep beep\n\tevade \n", printed)
		reparsed = (Globals(printed)[0] cast MacroStatement).Body.Statements
		Assert.AreEqual("beep beep", (reparsed[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def WhitespaceAgnosticCommandLines():
		code = "rr n:\nif n == 1:\nsay one two\nelse:\nevade\nend\nend\nsay 3\n"
		globals = ParseWsa(code).Globals.Statements
		Assert.AreEqual(2, globals.Count)
		test = (globals[0] cast MacroStatement).Body.Statements[0] cast IfStatement
		Assert.AreEqual("one two", (test.TrueBlock.Statements[0] cast MacroStatement).VerbatimBody.Value)
		Assert.IsNull((test.FalseBlock.Statements[0] cast MacroStatement).VerbatimBody)
		Assert.IsNull((globals[1] cast MacroStatement).VerbatimBody)

	[Test]
	def BodyLocations():
		code = "sql foo(1,\n        2):\n\n\t\tselect 1\n\t\t  from t\n"
		macro = Globals(code)[0] cast MacroStatement
		Assert.AreEqual("\nselect 1\n  from t", macro.VerbatimBody.Value)
		AssertAt(3, 9, macro.VerbatimBody.LexicalInfo)
		AssertAt(4, 9, macro.VerbatimBodyLocation(1, 0))
		AssertAt(5, 11, macro.VerbatimBodyLocation(2, 2))

	[Test]
	def LineLocations():
		say = (Globals("rr:\n    say   it's beep\n")[0] cast MacroStatement).Body.Statements[0] cast MacroStatement
		Assert.AreEqual("it's beep", say.VerbatimBody.Value)
		AssertAt(2, 11, say.VerbatimBody.LexicalInfo)
		AssertAt(2, 16, say.VerbatimBodyLocation(0, 5))

	[Test]
	def CommandNameDeclaredInsideItsMacroIsBoo():
		code = "rr:\n    say as int\n    say, b = pair()\n"
		for statement in (Globals(code)[0] cast MacroStatement).Body.Statements:
			Assert.IsNotInstanceOf[of MacroStatement](statement)

	[Test]
	def ReaderMacroDecidesWhatIsVerbatim():
		settings = ParserSettings()
		settings.ReaderMacros["shell"] = PromptReader()
		code = "shell:\n    \$ ls -l\n    ls = 1\nshell \$ pwd\nshell x\n"
		globals = BooParser.ParseReader(settings, "code", StringReader(code)).Modules[0].Globals.Statements
		body = (globals[0] cast MacroStatement).Body.Statements
		command = body[0] cast MacroStatement
		Assert.AreEqual("", command.Name)
		Assert.AreEqual("\$ ls -l", command.VerbatimBody.Value)
		Assert.IsInstanceOf[of ExpressionStatement](body[1])
		Assert.AreEqual("\$ pwd", (globals[1] cast MacroStatement).VerbatimBody.Value)
		Assert.IsNull((globals[2] cast MacroStatement).VerbatimBody)

	[Test]
	def NamespacedReaderMacroAppliesOnceImported():
		code = "import System\nimport Shells\nshell \$ x\n"
		macro = ParseNamespaced(code, "Shells").Globals.Statements[0] cast MacroStatement
		Assert.AreEqual("\$ x", macro.VerbatimBody.Value)

	[Test]
	def NamespacedReaderMacroIgnoresOtherImports():
		for code in ("import Shells as S\nshell \$ x\n", "import Shells(run)\nshell \$ x\n", "import Shells.More\nshell \$ x\n", "shell \$ x\n"):
			macro = ParseNamespaced(code, "Shells").Globals.Statements[0] cast MacroStatement
			Assert.IsNull(macro.VerbatimBody, code)

	[Test]
	def NamespacedReaderMacroAppliesAfterImportFrom():
		code = "import Shells from \"shells.dll\"\nshell \$ x\n"
		macro = ParseNamespaced(code, "Shells").Globals.Statements[0] cast MacroStatement
		Assert.AreEqual("\$ x", macro.VerbatimBody.Value)

	[Test]
	def NamespacedReaderMacroAppliesInItsOwnNamespace():
		code = "namespace Shells\nshell \$ x\n"
		macro = ParseNamespaced(code, "Shells").Globals.Statements[0] cast MacroStatement
		Assert.AreEqual("\$ x", macro.VerbatimBody.Value)

	[Test]
	def GlobalNamespaceReaderMacroNeedsNoImport():
		macro = ParseNamespaced("shell \$ x\n", "Boo.Lang.Extensions").Globals.Statements[0] cast MacroStatement
		Assert.AreEqual("\$ x", macro.VerbatimBody.Value)

	[Test]
	def ImportEndingInASemicolon():
		macro = ParseNamespaced("import Shells;\nshell \$ x\n", "Shells").Globals.Statements[0] cast MacroStatement
		Assert.AreEqual("\$ x", macro.VerbatimBody.Value)

	[Test]
	def ReaderMacroNotReadingLinesLeavesThemToTheEnclosingOne():
		settings = ParserSettings()
		settings.ReaderMacros["commands"] = ReaderMacro.CommandBlock
		settings.ReaderMacros["once"] = OneLineReader()
		code = "commands:\n    once:\n        ls -l\n"
		commands = BooParser.ParseReader(settings, "code", StringReader(code)).Modules[0].Globals.Statements[0] cast MacroStatement
		command = (commands.Body.Statements[0] cast MacroStatement).Body.Statements[0] cast MacroStatement
		Assert.AreEqual("", command.Name)
		Assert.AreEqual("ls -l", command.VerbatimBody.Value)

	[Test]
	def VerbatimMacroDefinedInTheSameFile():
		code = "macro foo(text as verbatim):\n    pass\nfoo it's \"odd\nfoo:\n    one\n"
		globals = ParseUnregistered(code).Globals.Statements
		Assert.AreEqual("it's \"odd", (globals[1] cast MacroStatement).VerbatimBody.Value)
		Assert.AreEqual("one", (globals[2] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def VerbatimArgumentInParentheses():
		code = "macro foo((text as verbatim)):\n    pass\nfoo it's\n"
		Assert.AreEqual("it's", (ParseUnregistered(code).Globals.Statements[1] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def NestedVerbatimMacroDefinedInTheSameFile():
		code = "macro foo:\n    macro bar(text as verbatim):\n        pass\n    yield foo.Body\nfoo:\n    bar it's\nbar 1\n"
		globals = ParseUnregistered(code).Globals.Statements
		Assert.AreEqual("it's", ((globals[1] cast MacroStatement).Body.Statements[0] cast MacroStatement).VerbatimBody.Value)
		Assert.IsNull((globals[2] cast MacroStatement).VerbatimBody)

	[Test]
	def ExtensionVerbatimMacroDefinedInTheSameFile():
		code = "macro foo:\n    yield foo.Body\nmacro foo.bar(text as verbatim):\n    pass\nfoo:\n    bar it's\n"
		globals = ParseUnregistered(code).Globals.Statements
		Assert.AreEqual("it's", ((globals[2] cast MacroStatement).Body.Statements[0] cast MacroStatement).VerbatimBody.Value)

	[Test]
	def NestedDefinitionsBelongToTheirOwnParent():
		code = "macro foo:\n    macro bar(text as verbatim):\n        pass\nmacro baz:\n    macro qux(text as verbatim):\n        pass\nfoo:\n    qux 1\n"
		foo = ParseUnregistered(code).Globals.Statements[2] cast MacroStatement
		qux = foo.Body.Statements[0] cast MacroStatement
		Assert.IsNull(qux.VerbatimBody)
		Assert.AreEqual(1, qux.Arguments.Count)

	[Test]
	def VerbatimMacroWithOtherArgumentsTakesOnlyABlock():
		code = "macro foo(n as int, text as verbatim):\n    pass\nfoo 1:\n    it's\nfoo 2\n"
		globals = ParseUnregistered(code).Globals.Statements
		block = globals[1] cast MacroStatement
		Assert.AreEqual(1, block.Arguments.Count)
		Assert.AreEqual("it's", block.VerbatimBody.Value)
		line = globals[2] cast MacroStatement
		Assert.AreEqual(1, line.Arguments.Count)
		Assert.IsNull(line.VerbatimBody)

	[Test]
	def MacroWithoutAVerbatimArgumentTakesBoo():
		code = "macro foo(text as string):\n    pass\nfoo:\n    x = 1\n"
		foo = ParseUnregistered(code).Globals.Statements[1] cast MacroStatement
		Assert.IsNull(foo.VerbatimBody)
		Assert.AreEqual(1, foo.Body.Statements.Count)

	[Test]
	def WhitespaceAgnosticVerbatimMacroDefinedInTheSameFile():
		code = "macro foo:\nmacro bar(text as verbatim):\npass\nend\nyield foo.Body\nend\nfoo:\nbar it's\nend\nbar 1\n"
		globals = ParseWsa(code).Globals.Statements
		Assert.AreEqual(3, globals.Count)
		Assert.AreEqual("it's", ((globals[1] cast MacroStatement).Body.Statements[0] cast MacroStatement).VerbatimBody.Value)
		Assert.IsNull((globals[2] cast MacroStatement).VerbatimBody)

	private static def ParseUnregistered(code as string) as Module:
		unit = BooParser.ParseReader(ParserSettings(), "code", StringReader(code))
		return unit.Modules[0]

	private static def ParseNamespaced(code as string, ns as string) as Module:
		settings = ParserSettings()
		macros = System.Collections.Generic.Dictionary[of string, ReaderMacro]()
		macros["shell"] = PromptReader()
		settings.ReaderMacrosByNamespace[ns] = macros
		return BooParser.ParseReader(settings, "code", StringReader(code)).Modules[0]

	private static def AssertAt(line as int, column as int, location as LexicalInfo):
		Assert.AreEqual("code", location.FileName)
		Assert.AreEqual(line, location.Line, "line")
		Assert.AreEqual(column, location.Column, "column")

	private static def ParseWsa(code as string) as Module:
		compiler = BooCompiler()
		compiler.Parameters.Pipeline = CompilerPipeline()
		compiler.Parameters.Pipeline.Add(WSABooParsingStep())
		compiler.Parameters.Environment = ClosedEnvironment(Settings())
		compiler.Parameters.Input.Add(StringInput("code", code))
		result = compiler.Run()
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString())
		return result.CompileUnit.Modules[0]

	private static def Parse(code as string) as Module:
		unit = BooParser.ParseReader(Settings(), "code", StringReader(code))
		return unit.Modules[0]

	private static def Settings():
		settings = ParserSettings()
		settings.ReaderMacros["sql"] = ReaderMacro.VerbatimBlock
		nested = System.Collections.Generic.Dictionary[of string, ReaderMacro]()
		nested["say"] = ReaderMacro.Verbatim
		settings.ReaderMacros["rr"] = ReaderMacro.Nesting(nested)
		return settings

	private static def Globals(code as string):
		return Parse(code).Globals.Statements
