using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using NUnit.Framework;

namespace BooCompiler.Tests.TypeSystem.Services
{
    [TestFixture]
    public class DowncastPermissionsTest
    {
        class Base {}
        class Derived : Base {}
        interface IInterface {}

        [Test]
        public void RegularDowncastAllowedByDefault()
        {
        	RunInCompilerContextEnvironment(() =>
        	{   
        		var subject1 = My<DowncastPermissions>.Instance;
        		Assert.IsTrue(subject1.CanBeReachedByDowncast(ITypeFor<Derived>(), ITypeFor<Base>()));
        	});
        }

    	[Test]
        public void InterfaceDowncastAllowedByDefault()
        {
			RunInCompilerContextEnvironment(() =>
            {
                var subject = My<DowncastPermissions>.Instance;
                Assert.IsTrue(subject.CanBeReachedByDowncast(ITypeFor<Derived>(), ITypeFor<IInterface>()));
            });
        }

        [Test]
        public void InterfaceDowncastNotAllowedInStrictMode()
        {
			RunInCompilerContextEnvironment(() =>
            {
                My<CompilerParameters>.Instance.Strict = true;

                var subject = My<DowncastPermissions>.Instance;
                Assert.IsFalse(subject.CanBeReachedByDowncast(ITypeFor<Derived>(), ITypeFor<IInterface>()));
            });
        }

        [Test]
        public void ArrayDowncastIsNotAllowed()
        {
			RunInCompilerContextEnvironment(() =>
            {
                var subject = My<DowncastPermissions>.Instance;
                Assert.IsFalse(subject.CanBeReachedByDowncast(ITypeFor<string[]>(), ITypeFor<object[]>()));
            });
        }

		private void RunInCompilerContextEnvironment(Action action)
		{
			ActiveEnvironment.With(new CompilerContext().Environment, () => {
				action();
			});
		}

        private static IType ITypeFor<T>()
        {
            return My<TypeSystemServices>.Instance.Map(typeof(T));
        }
    }
}
