namespace BooCompiler.Tests.TypeSystem.Services

import System
import System.Collections.Generic
import Boo.Lang.Compiler
import Boo.Lang.Compiler.IO
import Boo.Lang.Compiler.Pipelines
import Boo.Lang.Compiler.Steps
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Core
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class NameResolutionServiceTest:
	private static def ResolveQualifiedName(qualifiedName as string) as IEntity:
		return Subject().ResolveQualifiedName(qualifiedName)

	private static def Subject() as NameResolutionService:
		return My[of NameResolutionService].Instance

	private static def MatchIgnoringCase(candidate as IEntity, name as string) as bool:
		return 0 == string.Compare(candidate.Name, name, true)

	[Test]
	def NameMatchingCanBeCustomized():
		parameters = CompilerParameters()
		code = """
l = []
l.ADD(42)
l.add(42)
print JOIN(l, ", ")
"""
		parameters.Input.Add(StringInput("code", code))
		parameters.Pipeline = ResolveExpressions()
		parameters.Pipeline.Insert(0, ActionStep(
			{ My[of NameResolutionService].Instance.EntityNameMatcher = MatchIgnoringCase }))
		result = Boo.Lang.Compiler.BooCompiler(parameters).Run()
		Assert.AreEqual(0, result.Errors.Count, result.Errors.ToString())

	[Test]
	def ResolveQualifiedName():
		RunInCompilerContextEnvironment() do:
			Subject().EnterNamespace(My[of GlobalNamespace].Instance)
			result as IEntity = ResolveQualifiedName("Boo.Lang")
			Assert.IsNotNull(result)
			Assert.AreEqual(EntityType.Namespace, result.EntityType)

			builtinsType = ResolveQualifiedName("Boo.Lang.Builtins") as IType
			Assert.IsNotNull(builtinsType)

	private class Named(IEntity):
		[Getter(Name)]
		_name as string

		def constructor(name as string):
			_name = name

		FullName as string:
			get: return _name

		EntityType as EntityType:
			get: return EntityType.Local

	private class Scope(INamespace):
		_parent as INamespace
		_names = Dictionary[of string, IEntity]()

		def constructor(parent as INamespace, *names as (string)):
			_parent = parent
			for name in names:
				_names[name] = Named("${name} of ${GetHashCode()}")

		Name as string:
			get: return "scope"

		FullName as string:
			get: return "scope"

		EntityType as EntityType:
			get: return EntityType.Namespace

		ParentNamespace as INamespace:
			get: return _parent

		def Resolve(resultingSet as ICollection[of IEntity], name as string, typesToConsider as EntityType) as bool:
			entity as IEntity
			return false unless _names.TryGetValue(name, entity)
			resultingSet.Add(entity)
			return true

		def GetMembers() as IEnumerable[of IEntity]:
			return _names.Values

		def Of(name as string) as IEntity:
			return _names[name]

	[Test]
	def RestoringAScopeLeavesTheNamesOfTheScopeBehindOutOfTheNextLeave():
		// A scope entered, and left by entering the one before it again, used to
		// leave its cache on the stack for the next LeaveNamespace to take for the
		// cache of the namespace it was returning to.
		RunInCompilerContextEnvironment() do:
			subject = Subject()
			outer = Scope(My[of GlobalNamespace].Instance, "x")
			block = Scope(outer)
			nested = Scope(My[of GlobalNamespace].Instance, "x")
			subject.EnterNamespace(outer)
			subject.EnterNamespace(block)
			Assert.AreSame(outer.Of("x"), subject.Resolve("x"))

			saved = subject.SaveScope()
			subject.EnterNamespace(nested)
			Assert.AreSame(nested.Of("x"), subject.Resolve("x"))
			subject.RestoreScope(saved)
			Assert.AreSame(block, subject.CurrentNamespace)
			Assert.AreSame(outer.Of("x"), subject.Resolve("x"))

			subject.LeaveNamespace()
			Assert.AreSame(outer, subject.CurrentNamespace)
			Assert.AreSame(outer.Of("x"), subject.Resolve("x"))

	[Test]
	def RestoringAScopeAlsoDropsWhatWasEnteredAndNeverLeft():
		RunInCompilerContextEnvironment() do:
			subject = Subject()
			outer = Scope(My[of GlobalNamespace].Instance, "x")
			stray = Scope(My[of GlobalNamespace].Instance, "x")
			subject.EnterNamespace(outer)
			saved = subject.SaveScope()
			subject.EnterNamespace(stray)
			subject.EnterNamespace(Scope(stray))
			subject.RestoreScope(saved)
			Assert.AreSame(outer, subject.CurrentNamespace)
			Assert.AreSame(outer.Of("x"), subject.Resolve("x"))

	private def RunInCompilerContextEnvironment(action as Action):
		ActiveEnvironment.With(CompilerContext().Environment):
			action()
