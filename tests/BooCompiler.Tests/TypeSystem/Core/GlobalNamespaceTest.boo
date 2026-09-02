namespace BooCompiler.Tests.TypeSystem.Core

import System
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Core
import Boo.Lang.Compiler.TypeSystem.Reflection
import Boo.Lang.Compiler.Util
import Boo.Lang.Environments
import BooCompiler.Tests.TypeSystem
import NUnit.Framework

[TestFixture]
class GlobalNamespaceTest(AbstractTypeSystemTest):
	private _subject as GlobalNamespace

	[SetUp]
	public override def SetUp():
		super.SetUp()

		Context.References.Add(typeof(Boo.Lang.List).Assembly)
		Context.References.Add(typeof(Boo.Lang.Compiler.CompilerContext).Assembly)

		RunInCompilerContextEnvironment({ _subject = My[of GlobalNamespace].Instance })

	[Test]
	def CompilerContextAssumptions():
		Assert.AreEqual(2, Context.References.Count)

	[Test]
	def ParentNamespace():
		Assert.IsNull(_subject.ParentNamespace)

	[Test]
	def ResolveTopLevelNamespace():
		RunInCompilerContextEnvironment() do:
			booCompiler = NamespaceAssert.ResolveSingle(_subject, "Boo") as INamespace
			Assert.AreEqual(EntityType.Namespace, booCompiler.EntityType)
			Assert.AreEqual("Boo", booCompiler.Name)
			Assert.AreEqual("Boo", booCompiler.FullName)
			Assert.AreSame(_subject, booCompiler.ParentNamespace)

	[Test]
	def ResolveNestedNamespace():
		RunInCompilerContextEnvironment() do:
			booLang = ResolveQualifiedNameToSingle("Boo.Lang") as INamespace
			Assert.AreEqual(EntityType.Namespace, booLang.EntityType)
			Assert.AreEqual("Lang", booLang.Name)
			Assert.AreEqual("Boo.Lang", booLang.FullName)
			Assert.AreEqual("Boo", booLang.ParentNamespace.Name)
			Assert.AreSame(_subject, booLang.ParentNamespace.ParentNamespace)

	[Test]
	def ResolveSingleType():
		RunInCompilerContextEnvironment() do:
			AssertSingleTypeResolution(typeof(Boo.Lang.Builtins))
			AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.CompilerContext))

	[Test]
	def ResolveAmbiguousGenericNonGenericType():
		RunInCompilerContextEnvironment() do:
			found as Set[of IEntity] = ResolveQualifiedName("Boo.Lang.List")
			Assert.AreEqual(2, found.Count, found.ToString())

	[Test]
	def ResolveSingleInternalType():
		RunInCompilerContextEnvironment() do:
			bazType as IType = DefineInternalClass("Foo.Bar", "Baz")
			AssertTypeResolution(bazType, "Foo.Bar.Baz")

	[Test]
	def SingleEnumType():
		RunInCompilerContextEnvironment({ AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.Ast.TypeMemberModifiers)) })

	[Test]
	def SingleEnumTypeWithInternalModuleInSiblingNamespace():
		RunInCompilerContextEnvironment() do:
			fooType as IType = DefineInternalClass("Boo.Lang.Compiler", "Foo")
			AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.Ast.TypeMemberModifiers))
			AssertTypeResolution(fooType, fooType.FullName)

	[Test]
	def InternalTypeWithSameNameAsReferencedType():
		RunInCompilerContextEnvironment() do:
			subjectType as Type = _subject.GetType()
			internalType as IType = DefineInternalClass(subjectType.Namespace, subjectType.Name)
			found as Set[of IEntity] = ResolveQualifiedName(subjectType.FullName)
			Assert.IsTrue(found.ContainsAll((Map(subjectType), internalType)))

	private def ResolveQualifiedName(qualifiedName as string) as Set[of IEntity]:
		return NamespaceAssert.ResolveQualifiedName(_subject, qualifiedName)

	private def AssertSingleTypeResolution(type as Type):
		AssertTypeResolution(Map(type), type.FullName)

	private static def Map(type as Type) as IType:
		return My[of IReflectionTypeSystemProvider].Instance.Map(type)

	private def AssertTypeResolution(expected as IType, typeFullName as string):
		resolved as IType = AssertSingleTypeResolution(typeFullName)
		Assert.AreEqual(typeFullName, expected.FullName)
		Assert.AreSame(expected, resolved)

	private def AssertSingleTypeResolution(typeFullName as string) as IType:
		found as IEntity = ResolveQualifiedNameToSingle(typeFullName)
		Assert.AreEqual(EntityType.Type, found.EntityType)
		Assert.AreEqual(typeFullName, found.FullName)
		return found as IType

	private def ResolveQualifiedNameToSingle(qualifiedName as string) as IEntity:
		return NamespaceAssert.ResolveQualifiedNameToSingle(_subject, qualifiedName)
