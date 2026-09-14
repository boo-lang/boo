namespace Boo.Lang.Parser.Tests

import NUnit.Framework
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Environments
import Boo.Lang.Parser

[TestFixture]
class CommandBlockTestFixture:
	"""
	Inside a macro registered as ReaderMacro.CommandBlock, a line is Boo
	when it clearly reads as Boo and a command line kept as text otherwise.
	"""
	[Test]
	def CommandLinesAreKeptAsText():
		code = "commands:\n    echo hello \$name\n    ./build.sh --fast\n    print it's \"open\n"
		body = Block(code)
		Assert.AreEqual(3, body.Count)
		AssertCommand("echo hello \$name", body[0])
		AssertCommand("./build.sh --fast", body[1])
		AssertCommand("print it's \"open", body[2])

	[Test]
	def BooLinesMixWithCommands():
		code = """commands:
    files = glob("*.boo")
    for file in files:
        echo \$file
    notify()
    log.Write(1)
    items[0].Run()
    a, b = pair()
    # a comment
    if files:
        pass
    else:
        git status --short
"""
		body = Block(code)
		Assert.AreEqual(7, body.Count)
		Assert.IsInstanceOf[of ExpressionStatement](body[0])
		AssertCommand("echo \$file", (body[1] cast ForStatement).Block.Statements[0])
		Assert.IsInstanceOf[of ExpressionStatement](body[2])
		Assert.IsInstanceOf[of ExpressionStatement](body[3])
		Assert.IsInstanceOf[of ExpressionStatement](body[4])
		Assert.IsInstanceOf[of UnpackStatement](body[5])
		AssertCommand("git status --short", (body[6] cast IfStatement).FalseBlock.Statements[0])

	[Test]
	def ArgumentsOnTheBlock():
		macro = Globals("commands ctx:\n    pwd\n")[0] cast MacroStatement
		Assert.AreEqual(1, macro.Arguments.Count)
		AssertCommand("pwd", macro.Body.Statements[0])

	[Test]
	def CommandsEndWithTheBlock():
		globals = Globals("echo 1\ncommands:\n    echo 2\necho 3\n")
		Assert.AreEqual(3, globals.Count)
		Assert.AreEqual("echo", (globals[0] cast MacroStatement).Name)
		AssertCommand("echo 2", (globals[1] cast MacroStatement).Body.Statements[0])
		Assert.AreEqual("echo", (globals[2] cast MacroStatement).Name)

	[Test]
	def InsideAMethod():
		code = "def f():\n    commands:\n        echo 1\n    print 2\n"
		method = Parse(code, BooParsingStep()).Members[0] cast Method
		Assert.AreEqual(2, method.Body.Statements.Count)
		AssertCommand("echo 1", (method.Body.Statements[0] cast MacroStatement).Body.Statements[0])

	[Test]
	def WhitespaceAgnostic():
		code = "commands:\nif x:\necho one two\nelse:\nls\nend\nend\necho 3\n"
		globals = Parse(code, WSABooParsingStep()).Globals.Statements
		Assert.AreEqual(2, globals.Count)
		test = (globals[0] cast MacroStatement).Body.Statements[0] cast IfStatement
		AssertCommand("echo one two", test.TrueBlock.Statements[0])
		AssertCommand("ls", test.FalseBlock.Statements[0])
		Assert.AreEqual("echo", (globals[1] cast MacroStatement).Name)

	[Test]
	def PrinterWritesCommandLinesBack():
		code = "commands:\n    echo hello \$name\n    x = 1\n"
		printed = (Globals(code)[0] cast MacroStatement).ToCodeString().Replace("\r\n", "\n")
		Assert.AreEqual("commands :\n\techo hello \$name\n\tx = 1\n", printed)
		AssertCommand("echo hello \$name", (Globals(printed)[0] cast MacroStatement).Body.Statements[0])

	[Test]
	def OneLineCommand():
		globals = Globals("commands git status --short\ntry:\n    commands echo it's\nexcept:\n    pass\n")
		AssertOneLine("git status --short", globals[0])
		AssertOneLine("echo it's", (globals[1] cast TryStatement).ProtectedBlock.Statements[0])

	[Test]
	def CommandLocations():
		globals = Globals("commands   git status\ncommands:\n    ls -l\n")
		one = (globals[0] cast MacroStatement).VerbatimBody.LexicalInfo
		Assert.AreEqual("1:12", "${one.Line}:${one.Column}")
		line = ((globals[1] cast MacroStatement).Body.Statements[0] cast MacroStatement).VerbatimBody.LexicalInfo
		Assert.AreEqual("3:5", "${line.Line}:${line.Column}")

	[Test]
	def OtherUsesOfTheNameStayBoo():
		code = "commands(\"git\", \"status\")\ncommands = 1\ncommands.Run()\ncommands ctx:\n    ls\ncommands\n"
		globals = Globals(code)
		Assert.AreEqual(5, globals.Count)
		Assert.IsInstanceOf[of ExpressionStatement](globals[0])
		Assert.IsInstanceOf[of ExpressionStatement](globals[1])
		Assert.IsInstanceOf[of ExpressionStatement](globals[2])
		AssertCommand("ls", (globals[3] cast MacroStatement).Body.Statements[0])
		Assert.IsFalse((globals[4] cast MacroStatement).IsVerbatimLine)

	[Test]
	def OneLineCommandInsideABlock():
		body = Block("commands:\n    commands git status\n    ls\n")
		AssertOneLine("git status", body[0])
		AssertCommand("ls", body[1])

	[Test]
	def WhitespaceAgnosticOneLineCommand():
		globals = Parse("if true:\ncommands git status\nend\nprint 1\n", WSABooParsingStep()).Globals.Statements
		Assert.AreEqual(2, globals.Count)
		AssertOneLine("git status", (globals[0] cast IfStatement).TrueBlock.Statements[0])

	[Test]
	def PrinterWritesOneLineCommandsBack():
		printed = (Globals("commands git status \$x\n")[0] cast MacroStatement).ToCodeString().Replace("\r\n", "\n")
		Assert.AreEqual("commands git status \$x\n", printed)
		AssertOneLine("git status \$x", Globals(printed)[0])

	private static def AssertOneLine(text as string, statement as Statement):
		command = statement as MacroStatement
		Assert.IsNotNull(command, "expected a one line command, got ${statement}")
		Assert.AreEqual("commands", command.Name)
		Assert.IsTrue(command.IsVerbatimLine)
		Assert.AreEqual(text, command.VerbatimBody.Value)
		Assert.IsTrue(command.Body.IsEmpty)

	private static def AssertCommand(text as string, statement as Statement):
		command = statement as MacroStatement
		Assert.IsNotNull(command, "expected a command line, got ${statement}")
		Assert.AreEqual("", command.Name)
		Assert.IsTrue(command.IsVerbatimLine)
		Assert.AreEqual(text, command.VerbatimBody.Value)

	private static def Block(code as string):
		return (Globals(code)[0] cast MacroStatement).Body.Statements

	private static def Globals(code as string):
		return Parse(code, BooParsingStep()).Globals.Statements

	private static def Parse(code as string, step as ICompilerStep) as Module:
		compiler = BooCompiler()
		compiler.Parameters.Pipeline = CompilerPipeline()
		compiler.Parameters.Pipeline.Add(step)
		settings = ParserSettings()
		settings.ReaderMacros["commands"] = ReaderMacro.CommandBlock
		compiler.Parameters.Environment = ClosedEnvironment(settings)
		compiler.Parameters.Input.Add(StringInput("code", code))
		result = compiler.Run()
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString())
		return result.CompileUnit.Modules[0]
