namespace BooCompiler.Tests

import System
import System.IO
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import NUnit.Framework

[TestFixture]
class JsonMetaMethodTest:
"""
The Json meta method, which writes a Boo literal as a System.Text.Json tree.

The source is compiled and run, since what matters is the JSON that comes
out, not the shape of the tree the meta method returned.
"""

	private def Run(code as string) as string:
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput("json", code))
		compiler.Parameters.Pipeline = CompileToMemory()
		compiler.Parameters.References.Add(typeof(Boo.Lang.Extensions.PrintMacro).Assembly)
		compiler.Parameters.References.Add(typeof(System.Text.Json.Nodes.JsonNode).Assembly)
		context = compiler.Run()
		Assert.AreEqual(0, context.Errors.Count, Verbose(context.Errors))

		written = StringWriter()
		before = Console.Out
		Console.SetOut(written)
		try:
			context.GeneratedAssembly.EntryPoint.Invoke(null, (null,))
		ensure:
			Console.SetOut(before)
		return written.ToString().Trim()

	private def Verbose(errors as Boo.Lang.Compiler.CompilerErrorCollection) as string:
		written = System.Text.StringBuilder()
		for error as Boo.Lang.Compiler.CompilerError in errors:
			written.AppendLine(error.ToString(true))
			inner = error.InnerException
			while inner is not null:
				written.AppendLine("  inner: ${inner.GetType().Name}: ${inner.Message}")
				written.AppendLine("  at: ${inner.StackTrace}")
				inner = inner.InnerException
		return written.ToString()

	private def Errors(code as string) as string:
		compiler = BooCompiler()
		compiler.Parameters.Input.Add(StringInput("json", code))
		compiler.Parameters.Pipeline = ResolveExpressions(BreakOnErrors: false)
		compiler.Parameters.References.Add(typeof(Boo.Lang.Extensions.PrintMacro).Assembly)
		compiler.Parameters.References.Add(typeof(System.Text.Json.Nodes.JsonNode).Assembly)
		return compiler.Run().Errors.ToString()

	[Test]
	def WritesAHashLiteralAsAnObject():
		assert '{"a":1}' == Run("print Json({'a': 1}).ToJsonString()")

	[Test]
	def WritesAListLiteralAsAnArray():
		assert '[1,2,3]' == Run("print Json([1, 2, 3]).ToJsonString()")

	[Test]
	def WritesWhatIsNested():
		assert '{"a":{"b":[1]}}' == Run("print Json({'a': {'b': [1]}}).ToJsonString()")

	[Test]
	def WritesAnEmptyObjectAndArray():
		assert '{}' == Run("print Json({}).ToJsonString()")
		assert '[]' == Run("print Json([]).ToJsonString()")

	[Test]
	def WritesTheKindsOfValueJsonHas():
		assert '{"s":"x","n":2,"b":true}' == Run("print Json({'s': 'x', 'n': 2, 'b': true}).ToJsonString()")

	[Test]
	def RefusesAKeyThatIsNotAString():
		assert "A JSON key has to be a string." in Errors("print Json({1: 'a'}).ToJsonString()")

	[Test]
	def TakesANodeSomethingElseAlreadyBuilt():
		code = """
import System.Text.Json.Nodes
inner = JsonObject()
inner['b'] = 1
print Json({'a': inner}).ToJsonString()
"""
		assert '{"a":{"b":1}}' == Run(code)

	[Test]
	def TakesANodeInsideAnArray():
		code = """
import System.Text.Json.Nodes
print Json([JsonArray(), 2]).ToJsonString()
"""
		assert '[[],2]' == Run(code)
