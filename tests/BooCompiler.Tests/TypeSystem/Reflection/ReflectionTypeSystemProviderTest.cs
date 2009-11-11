using Boo.Lang;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using BooCompiler.Tests.TypeSystem.Core;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Reflection
{
	[TestFixture]
	public class ReflectionTypeSystemProviderTest
	{
		private readonly IReflectionTypeSystemProvider subject = new ReflectionTypeSystemProvider();

		[Test]
		public void ReferencesToSameAssemblyAreEqual()
		{
			ICompileUnit ref1 = subject.ForAssembly(GetType().Assembly);
			ICompileUnit ref2 = subject.ForAssembly(GetType().Assembly);
			Assert.IsNotNull(ref1);
			Assert.IsNotNull(ref2);
			Assert.AreEqual(ref1, ref2);
		}

		[Test]
		public void RootNamespace()
		{
			ICompileUnit reference = subject.ForAssembly(GetType().Assembly);
			INamespace root = reference.RootNamespace;

			Assert.IsFalse(root.Resolve(new List<IEntity>(), "XXX", EntityType.Any));

			IEntity type = NamespaceAssert.ResolveQualifiedNameToSingle(root, GetType().FullName);
			Assert.AreEqual(EntityType.Type, type.EntityType);
			Assert.AreEqual(type.FullName, GetType().FullName);
		}

		[Test]
		public void Clone()
		{
			ICompileUnit original = subject.ForAssembly(GetType().Assembly);
			IReflectionTypeSystemProvider clone = subject.Clone();
			Assert.AreNotSame(subject, clone);
			ICompileUnit referenceFromClone = clone.ForAssembly(GetType().Assembly);
			Assert.AreSame(original, referenceFromClone);
		}

		[Test]
		public void AssemblyReferenceExposesAssembly()
		{
			IAssemblyReference assemblyRef = subject.ForAssembly(GetType().Assembly) as IAssemblyReference;
			Assert.IsNotNull(assemblyRef);
			Assert.AreSame(GetType().Assembly, assemblyRef.Assembly);
		}
	}
}