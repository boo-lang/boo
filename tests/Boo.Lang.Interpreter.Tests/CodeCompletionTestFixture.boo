namespace Boo.Lang.Interpreter.Tests

import System.Collections.Generic
import Boo.Lang.Compiler
import Boo.Lang.Compiler.Ast
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Environments
import Boo.Lang.Interpreter
import NUnit.Framework

[TestFixture]
class CodeCompletionTestFixture:
"""
What the marker resolves to, and which of its members are worth offering.

The suggestions are taken straight from CodeCompletion rather than through an
interpreter, because an interpreter always asks without a scope.
"""

	private static final Inside = """
class Greeter:
	public def Pub():
		pass

	private def Priv():
		pass

	protected def Prot():
		pass

	def Use():
		self.__codecomplete__
"""

	private static final Outside = """
class Greeter:
	public def Pub():
		pass

	private def Priv():
		pass

	protected def Prot():
		pass

class Other:
	def Use(g as Greeter):
		g.__codecomplete__
"""

	private def Resolve(code as string):
		compiler = BooCompiler()
		pipeline = ResolveExpressions(BreakOnErrors: false)
		pipeline.Add(FindCodeCompleteSuggestion())
		compiler.Parameters.Pipeline = pipeline
		compiler.Parameters.Input.Add(StringInput("completion", code))
		return compiler.Run()

	private def Offered(code as string, scoped as bool) as List[of string]:
		result = Resolve(code)
		entity = result["suggestion"] as IEntity
		assert entity is not null, "the marker resolved to nothing"
		scope as TypeDefinition
		scope = result["scope"] as TypeDefinition if scoped
		names = List[of string]()
		# Inside the compiler's environment, which resolving a member needs.
		ActiveEnvironment.With(result.Environment) do:
			for item in CodeCompletion.SuggestionsFor(entity, false, scope):
				names.Add(item.Name)
		return names

	[Test]
	def OffersWhatTheCursorCanReachFromInsideTheClass():
		names = Offered(Inside, true)
		assert "Pub" in names
		assert "Priv" in names
		assert "Prot" in names

	[Test]
	def HidesWhatTheCursorCannotReachFromAnotherClass():
		names = Offered(Outside, true)
		assert "Pub" in names
		assert "Priv" not in names
		assert "Prot" not in names

	[Test]
	def OffersOnlyThePublicSurfaceWithoutAScope():
		names = Offered(Inside, false)
		assert "Pub" in names
		assert "Priv" not in names
