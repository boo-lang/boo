using System.Collections.Generic;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Services;
using BooCompiler.Tests.TypeSystem.Core;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Internal
{
	[TestFixture]
	public class InternalCompileUnitTest : AbstractTypeSystemTest
	{
		private ICompileUnit subject;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			context.Run(delegate
			{	
				subject = My<InternalTypeSystemProvider>.Instance.EntityFor(context.CompileUnit);
			});
		}

		[Test]
		public void EmptyCompileUnitHasNoMembers()
		{
			context.Run(delegate
			{
				Assert.IsTrue(IsEmpty(subject.RootNamespace.GetMembers()));
			});
		}

		private bool IsEmpty(IEnumerable<IEntity> source)
		{
			return !source.GetEnumerator().MoveNext();
		}

		[Test]
		public void ParentNamespaceIsTheGlobalNamespace()
		{
			context.Run(delegate
			{
				Assert.AreSame(My<NameResolutionService>.Instance.GlobalNamespace, subject.RootNamespace.ParentNamespace);
			});
		}

		[Test]
		public void SingleTypeResolutionForNamespaceWithTwoComponents()
		{
			context.Run(delegate
			{
				IType bazType = DefineInternalClass("Foo.Bar", "Baz");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionForNamespaceWithThreeComponents()
		{
			context.Run(delegate
			{
				IType bazType = DefineInternalClass("Foo.Bar.Zeng", "Baz");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionForSimpleNamespace()
		{
			context.Run(delegate
			{
				IType bazType = DefineInternalClass("Foo", "Baz");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionNoNamespace()
		{
			context.Run(delegate
			{
				IType bazType = DefineInternalClass("", "Baz");
				Assert.AreEqual("Baz", bazType.FullName);
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionAgainstTwoModules()
		{
			context.Run(delegate
			{
				IType bazType = DefineInternalClass("Foo.Bar", "Baz");
				IType eggsType = DefineInternalClass("Spam", "Eggs");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(subject.RootNamespace, bazType.FullName));
				Assert.AreSame(eggsType, NamespaceAssert.ResolveQualifiedNameToSingle(subject.RootNamespace, eggsType.FullName));
			});
		}

		[Test]
		public void ModuleNamespace()
		{
			context.Run(delegate
			{
				DefineInternalClass("Foo", "Bar");
				IEntity entity = NamespaceAssert.ResolveSingle(subject.RootNamespace, "Foo");
				Assert.AreEqual(EntityType.Namespace, entity.EntityType);
			});
		}

		[Test]
		public void ModuleNamespaceParent()
		{
			context.Run(delegate
			{
				IType bazType = DefineInternalClass("Foo.Bar", "Baz");
				INamespace fooNamespace = (INamespace) NamespaceAssert.ResolveSingle(subject.RootNamespace, "Foo");
				Assert.AreEqual(EntityType.Namespace, fooNamespace.EntityType);
				Assert.AreSame(My<NameResolutionService>.Instance.GlobalNamespace, fooNamespace.ParentNamespace);
				
				IEntity barNamespace = NamespaceAssert.ResolveSingle(fooNamespace, "Bar");
				Assert.AreEqual(EntityType.Namespace, fooNamespace.EntityType);
				Assert.AreSame(fooNamespace, ((INamespace)barNamespace).ParentNamespace);
			});
		}
	}
}
