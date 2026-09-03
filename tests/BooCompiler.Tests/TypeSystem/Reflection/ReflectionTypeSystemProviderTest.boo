namespace BooCompiler.Tests.TypeSystem.Reflection

import System
import Boo.Lang
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Reflection
import BooCompiler.Tests.TypeSystem.Core
import NUnit.Framework

[TestFixture]
class ReflectionTypeSystemProviderTest:
	private final _subject as IReflectionTypeSystemProvider = ReflectionTypeSystemProvider()

	[Test]
	def ReferencesToSameAssemblyAreEqual():
		ref1 as ICompileUnit = _subject.ForAssembly(GetType().Assembly)
		ref2 as ICompileUnit = _subject.ForAssembly(GetType().Assembly)
		Assert.IsNotNull(ref1)
		Assert.IsNotNull(ref2)
		Assert.AreEqual(ref1, ref2)

	[Test]
	def RootNamespace():
		reference as ICompileUnit = _subject.ForAssembly(GetType().Assembly)
		root as INamespace = reference.RootNamespace

		Assert.IsFalse(root.Resolve(Boo.Lang.List[of IEntity](), "XXX", EntityType.Any))

		type as IEntity = NamespaceAssert.ResolveQualifiedNameToSingle(root, GetType().FullName)
		Assert.AreEqual(EntityType.Type, type.EntityType)
		Assert.AreEqual(type.FullName, GetType().FullName)

	[Test]
	def ClonePreservesOriginalReferences():
		original as ICompileUnit = _subject.ForAssembly(GetType().Assembly)
		clone as IReflectionTypeSystemProvider = _subject.Clone()
		Assert.AreNotSame(_subject, clone)
		referenceFromClone as ICompileUnit = clone.ForAssembly(GetType().Assembly)
		Assert.AreSame(original, referenceFromClone)

	[Test]
	def AssemblyReferenceExposesAssembly():
		assemblyRef = _subject.ForAssembly(GetType().Assembly)
		Assert.IsNotNull(assemblyRef)
		Assert.AreSame(GetType().Assembly, assemblyRef.Assembly)

	[Test]
	def CloningPreservesTypeIdentityAccrossProviders():
		type = typeof(int)
		_subject.ForAssembly(type.Assembly)
		clone as IReflectionTypeSystemProvider = _subject.Clone()
		Assert.AreSame(_subject.Map(type), clone.Map(type))
