using System.Collections.Generic;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Internal;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using BooCompiler.Tests.TypeSystem.Core;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Internal
{
	[TestFixture]
	public class InternalCompileUnitTest : AbstractTypeSystemTest
	{
		private ICompileUnit _subject;

		[SetUp]
		public override void SetUp()
		{
			base.SetUp();
			RunInCompilerContextEnvironment(() =>
			{	
				_subject = My<InternalTypeSystemProvider>.Instance.EntityFor(Context.CompileUnit);
			});
		}

		[Test]
		public void EmptyCompileUnitHasNoMembers()
		{
			RunInCompilerContextEnvironment(() => Assert.IsTrue(IsEmpty(_subject.RootNamespace.GetMembers())));
		}

		private static bool IsEmpty(IEnumerable<IEntity> source)
		{
			return !source.GetEnumerator().MoveNext();
		}

		[Test]
		public void ParentNamespaceIsTheGlobalNamespace()
		{
			RunInCompilerContextEnvironment(
				() => Assert.AreSame(My<NameResolutionService>.Instance.GlobalNamespace, _subject.RootNamespace.ParentNamespace));
		}

		[Test]
		public void SingleTypeResolutionForNamespaceWithTwoComponents()
		{
			RunInCompilerContextEnvironment(() =>
			{
				IType bazType = DefineInternalClass("Foo.Bar", "Baz");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionForNamespaceWithThreeComponents()
		{
			RunInCompilerContextEnvironment(() =>
			{
				IType bazType = DefineInternalClass("Foo.Bar.Zeng", "Baz");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionForSimpleNamespace()
		{
			RunInCompilerContextEnvironment(() =>
			{
				IType bazType = DefineInternalClass("Foo", "Baz");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionNoNamespace()
		{
			RunInCompilerContextEnvironment(() =>
			{
				IType bazType = DefineInternalClass("", "Baz");
				Assert.AreEqual("Baz", bazType.FullName);
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName));
			});
		}

		[Test]
		public void SingleTypeResolutionAgainstTwoModules()
		{
			RunInCompilerContextEnvironment(() =>
			{
				IType bazType = DefineInternalClass("Foo.Bar", "Baz");
				IType eggsType = DefineInternalClass("Spam", "Eggs");
				Assert.AreSame(bazType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, bazType.FullName));
				Assert.AreSame(eggsType, NamespaceAssert.ResolveQualifiedNameToSingle(_subject.RootNamespace, eggsType.FullName));
			});
		}

		[Test]
		public void ModuleNamespace()
		{
			RunInCompilerContextEnvironment(() =>
			{
				DefineInternalClass("Foo", "Bar");
				IEntity entity = NamespaceAssert.ResolveSingle(_subject.RootNamespace, "Foo");
				Assert.AreEqual(EntityType.Namespace, entity.EntityType);
			});
		}

		[Test]
		public void ModuleNamespaceParent()
		{
			RunInCompilerContextEnvironment(() =>
			{
				DefineInternalClass("Foo.Bar", "Baz");

				var fooNamespace = (INamespace) NamespaceAssert.ResolveSingle(_subject.RootNamespace, "Foo");
				Assert.AreEqual(EntityType.Namespace, fooNamespace.EntityType);
				Assert.AreSame(My<NameResolutionService>.Instance.GlobalNamespace, fooNamespace.ParentNamespace);
				
				var barNamespace = NamespaceAssert.ResolveSingle(fooNamespace, "Bar");
				Assert.AreEqual(EntityType.Namespace, fooNamespace.EntityType);
				Assert.AreSame(fooNamespace, ((INamespace)barNamespace).ParentNamespace);
			});
		}
	}
}
