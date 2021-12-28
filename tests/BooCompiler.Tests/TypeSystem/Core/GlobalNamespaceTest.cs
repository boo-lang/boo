using System;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Core
{
	[TestFixture]
	public class GlobalNamespaceTest : AbstractTypeSystemTest
	{
		private GlobalNamespace _subject;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			Context.References.Add(typeof(Boo.Lang.List).Assembly);
			Context.References.Add(typeof(Boo.Lang.Compiler.CompilerContext).Assembly);

			RunInCompilerContextEnvironment(() => _subject = My<GlobalNamespace>.Instance);
		}

		[Test]
		public void CompilerContextAssumptions()
		{
#if NET
			Assert.AreEqual(6, Context.References.Count);
#else
			Assert.AreEqual(2, Context.References.Count);
#endif
		}

		[Test]
		public void ParentNamespace()
		{
			Assert.IsNull(_subject.ParentNamespace);
		}
			
		[Test]
		public void ResolveTopLevelNamespace()
		{	
			RunInCompilerContextEnvironment(delegate
			{
				var booCompiler = (INamespace) NamespaceAssert.ResolveSingle(_subject, "Boo");
				Assert.AreEqual(EntityType.Namespace, booCompiler.EntityType);
				Assert.AreEqual("Boo", booCompiler.Name);
				Assert.AreEqual("Boo", booCompiler.FullName);
				Assert.AreSame(_subject, booCompiler.ParentNamespace);
			});
		}

		[Test]
		public void ResolveNestedNamespace()
		{
			RunInCompilerContextEnvironment(delegate
			{
				var booLang = (INamespace) ResolveQualifiedNameToSingle("Boo.Lang");
				Assert.AreEqual(EntityType.Namespace, booLang.EntityType);
				Assert.AreEqual("Lang", booLang.Name);
				Assert.AreEqual("Boo.Lang", booLang.FullName);
				Assert.AreEqual("Boo", booLang.ParentNamespace.Name);
				Assert.AreSame(_subject, booLang.ParentNamespace.ParentNamespace);
			});
		}

		[Test]
		public void ResolveSingleType()
		{
			RunInCompilerContextEnvironment(delegate
			{
				AssertSingleTypeResolution(typeof(Boo.Lang.Builtins));
				AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.CompilerContext));
			});
		}

		[Test]
		public void ResolveAmbiguousGenericNonGenericType()
		{
			RunInCompilerContextEnvironment(delegate
			{
				Set<IEntity> found = ResolveQualifiedName("Boo.Lang.List");
				Assert.AreEqual(2, found.Count, found.ToString());
			});
		}

		[Test]
		public void ResolveSingleInternalType()
		{
			RunInCompilerContextEnvironment(delegate
			{
				IType bazType = DefineInternalClass("Foo.Bar", "Baz");
				AssertTypeResolution(bazType, "Foo.Bar.Baz");
			});
		}

		[Test]
		public void SingleEnumType()
		{
			RunInCompilerContextEnvironment(() => AssertSingleTypeResolution(typeof (Boo.Lang.Compiler.Ast.TypeMemberModifiers)));
		}

		[Test]
		public void SingleEnumTypeWithInternalModuleInSiblingNamespace()
		{
			RunInCompilerContextEnvironment(delegate
			{
				IType fooType = DefineInternalClass("Boo.Lang.Compiler", "Foo");
				AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.Ast.TypeMemberModifiers));
				AssertTypeResolution(fooType, fooType.FullName);
			});
		}

		[Test]
		public void InternalTypeWithSameNameAsReferencedType()
		{
			RunInCompilerContextEnvironment(delegate
			{
				Type subjectType = _subject.GetType();
				IType internalType = DefineInternalClass(subjectType.Namespace, subjectType.Name);
				Set<IEntity> found = ResolveQualifiedName(subjectType.FullName);
				Assert.IsTrue(found.ContainsAll(new[] { Map(subjectType), internalType }));
			});
		}

		private Set<IEntity> ResolveQualifiedName(string qualifiedName)
		{
			return NamespaceAssert.ResolveQualifiedName(_subject, qualifiedName);
		}

		private void AssertSingleTypeResolution(Type type)
		{
			AssertTypeResolution(Map(type), type.FullName);
		}

		private static IType Map(Type type)
		{
			return My<IReflectionTypeSystemProvider>.Instance.Map(type);
		}

		private void AssertTypeResolution(IType expected, string typeFullName)
		{
			IType resolved = AssertSingleTypeResolution(typeFullName);
			Assert.AreEqual(typeFullName, expected.FullName);
			Assert.AreSame(expected, resolved);
		}

		private IType AssertSingleTypeResolution(string typeFullName)
		{
			IEntity found = ResolveQualifiedNameToSingle(typeFullName);
			Assert.AreEqual(EntityType.Type, found.EntityType);
			Assert.AreEqual(typeFullName, found.FullName);
			return (IType) found;
		}

		private IEntity ResolveQualifiedNameToSingle(string qualifiedName)
		{
			return NamespaceAssert.ResolveQualifiedNameToSingle(_subject, qualifiedName);
		}
	}
}