using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Core;
using Boo.Lang.Compiler.TypeSystem.Reflection;
using Boo.Lang.Compiler.Util;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Core
{
	[TestFixture]
	public class GlobalNamespaceTest : AbstractTypeSystemTest
	{
		private GlobalNamespace subject;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();

			context.References.Add(typeof(Boo.Lang.List).Assembly);
			context.References.Add(typeof(Boo.Lang.Compiler.CompilerContext).Assembly);
			
			subject = new GlobalNamespace(context);
		}

		[Test]
		public void CompilerContextAssumptions()
		{
			Assert.AreEqual(2, context.References.Count);
		}

		[Test]
		public void ParentNamespace()
		{
			Assert.IsNull(subject.ParentNamespace);
		}
			
		[Test]
		public void ResolveTopLevelNamespace()
		{	
			context.Run(delegate
			{
				INamespace booCompiler = (INamespace) NamespaceAssert.ResolveSingle(subject, "Boo");
				Assert.AreEqual(EntityType.Namespace, booCompiler.EntityType);
				Assert.AreEqual("Boo", booCompiler.Name);
				Assert.AreEqual("Boo", booCompiler.FullName);
				Assert.AreSame(subject, booCompiler.ParentNamespace);
			});
		}

		[Test]
		public void ResolveNestedNamespace()
		{
			context.Run(delegate
			{
				INamespace booLang = (INamespace) ResolveQualifiedNameToSingle("Boo.Lang");
				Assert.AreEqual(EntityType.Namespace, booLang.EntityType);
				Assert.AreEqual("Lang", booLang.Name);
				Assert.AreEqual("Boo.Lang", booLang.FullName);
				Assert.AreEqual("Boo", booLang.ParentNamespace.Name);
				Assert.AreSame(subject, booLang.ParentNamespace.ParentNamespace);
			});
		}

		[Test]
		public void ResolveSingleType()
		{
			context.Run(delegate
			{
				AssertSingleTypeResolution(typeof(Boo.Lang.Builtins));
				AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.CompilerContext));
			});
		}

		[Test]
		public void ResolveAmbiguousGenericNonGenericType()
		{
			context.Run(delegate
			{
				Set<IEntity> found = ResolveQualifiedName("Boo.Lang.List");
				Assert.AreEqual(2, found.Count, found.ToString());
			});
		}

		[Test]
		public void ResolveSingleInternalType()
		{
			context.Run(delegate
			{
				IType bazType = DefineInternalClass("Foo.Bar", "Baz");
				AssertTypeResolution(bazType, "Foo.Bar.Baz");
			});
		}

		[Test]
		public void SingleEnumType()
		{
			context.Run(delegate
			{
				AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.Ast.TypeMemberModifiers));
			});
		}

		[Test]
		public void SingleEnumTypeWithInternalModuleInSiblingNamespace()
		{
			context.Run(delegate
			{
				IType fooType = DefineInternalClass("Boo.Lang.Compiler", "Foo");
				AssertSingleTypeResolution(typeof(Boo.Lang.Compiler.Ast.TypeMemberModifiers));
				AssertTypeResolution(fooType, fooType.FullName);
			});
		}

		[Test]
		public void InternalTypeWithSameNameAsReferencedType()
		{
			context.Run(delegate
			{
				Type subjectType = subject.GetType();
				IType internalType = DefineInternalClass(subjectType.Namespace, subjectType.Name);
				Set<IEntity> found = ResolveQualifiedName(subjectType.FullName);
				Assert.IsTrue(found.ContainsAll(new IType[] { Map(subjectType), internalType }));
			});
		}

		private Set<IEntity> ResolveQualifiedName(string qualifiedName)
		{
			return NamespaceAssert.ResolveQualifiedName(subject, qualifiedName);
		}

		private void AssertSingleTypeResolution(Type type)
		{
			AssertTypeResolution(Map(type), type.FullName);
		}

		private IType Map(Type type)
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
			return NamespaceAssert.ResolveQualifiedNameToSingle(subject, qualifiedName);
		}
	}
}