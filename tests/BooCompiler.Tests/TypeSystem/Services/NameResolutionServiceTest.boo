namespace BooCompiler.Tests.TypeSystem.Services

import System
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

	private def RunInCompilerContextEnvironment(action as Action):
		ActiveEnvironment.With(CompilerContext().Environment):
			action()
