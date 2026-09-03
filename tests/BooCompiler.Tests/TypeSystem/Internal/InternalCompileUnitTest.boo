namespace BooCompiler.Tests.TypeSystem.Internal

import System.Collections.Generic
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Internal
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import BooCompiler.Tests.TypeSystem
import BooCompiler.Tests.TypeSystem.Core
import NUnit.Framework

[TestFixture]
class InternalCompileUnitTest(AbstractTypeSystemTest):
	private _subject as ICompileUnit

	[SetUp]
	public override def SetUp():
		super.SetUp()
		RunInCompilerContextEnvironment() do:
			_subject = My[of InternalTypeSystemProvider].Instance.EntityFor(Context.CompileUnit)

	[Test]
	def EmptyCompileUnitHasNoMembers():
		RunInCompilerContextEnvironment({ Assert.IsTrue(IsEmpty(_subject.RootNamespace.GetMembers())) })

	private static def IsEmpty(source as IEnumerable[of IEntity]) as bool:
		return not source.GetEnumerator().MoveNext()

	[Test]
	def ParentNamespaceIsTheGlobalNamespace():
		RunInCompilerContextEnvironment(
			{ Assert.AreSame(My[of NameResolutionService].Instance.GlobalNamespace, _subject.RootNamespace.ParentNamespace) })

	[Test]
	def SingleTypeResolutionForNamespaceWithTwoComponents():
		RunInCompilerContextEnvironment() do:
			bazType as IType = DefineInternalClass("Foo.Bar", "Baz")
			Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName))

	[Test]
	def SingleTypeResolutionForNamespaceWithThreeComponents():
		RunInCompilerContextEnvironment() do:
			bazType as IType = DefineInternalClass("Foo.Bar.Zeng", "Baz")
			Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName))

	[Test]
	def SingleTypeResolutionForSimpleNamespace():
		RunInCompilerContextEnvironment() do:
			bazType as IType = DefineInternalClass("Foo", "Baz")
			Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName))

	[Test]
	def SingleTypeResolutionNoNamespace():
		RunInCompilerContextEnvironment() do:
			bazType as IType = DefineInternalClass("", "Baz")
			Assert.AreEqual("Baz", bazType.FullName)
			Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName))

	[Test]
	def SingleTypeResolutionAgainstTwoModules():
		RunInCompilerContextEnvironment() do:
			bazType as IType = DefineInternalClass("Foo.Bar", "Baz")
			eggsType as IType = DefineInternalClass("Spam", "Eggs")
			Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName))
			Assert.AreSame(eggsType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, eggsType.FullName))

	[Test]
	def ModuleNamespace():
		RunInCompilerContextEnvironment() do:
			DefineInternalClass("Foo", "Bar")
			entity as IEntity = NamespaceAssert.ResolveSingle(_subject.RootNamespace, "Foo")
			Assert.AreEqual(EntityType.Namespace, entity.EntityType)

	[Test]
	def ModuleNamespaceParent():
		RunInCompilerContextEnvironment() do:
			DefineInternalClass("Foo.Bar", "Baz")

			fooNamespace = NamespaceAssert.ResolveSingle(_subject.RootNamespace, "Foo") as INamespace
			Assert.AreEqual(EntityType.Namespace, fooNamespace.EntityType)
			Assert.AreSame(My[of NameResolutionService].Instance.GlobalNamespace, fooNamespace.ParentNamespace)

			barNamespace = NamespaceAssert.ResolveSingle(fooNamespace, "Bar")
			Assert.AreEqual(EntityType.Namespace, fooNamespace.EntityType)
			Assert.AreSame(fooNamespace, (barNamespace as INamespace).ParentNamespace)
