using Boo.Lang.Compiler;
using Boo.Lang.Compiler.TypeSystem;
using Boo.Lang.Compiler.TypeSystem.Services;
using Boo.Lang.Environments;
using NUnit.Framework;
using Environment = Boo.Lang.Environments.Environment;

namespace BooCompiler.Tests.TypeSystem.Services
{
    [TestFixture]
    public class DowncastPermissionsTest
    {
        class Base { }
        class Derived : Base { }
        interface IInterface {}

        [Test]
        public void RegularDowncastAllowedByDefault()
        {
            Environment.With(new CompilerContext(), () =>
            {   
                var subject = My<DowncastPermissions>.Instance;
                Assert.IsTrue(subject.CanBeReachedByDowncast(ITypeFor<Derived>(), ITypeFor<Base>()));
            });
        }

        [Test]
        public void InterfaceDowncastAllowedByDefault()
        {
            Environment.With(new CompilerContext(), () =>
            {
                var subject = My<DowncastPermissions>.Instance;
                Assert.IsTrue(subject.CanBeReachedByDowncast(ITypeFor<Derived>(), ITypeFor<IInterface>()));
            });
        }

        [Test]
        public void InterfaceDowncastNotAllowedInStrictMode()
        {
            Environment.With(new CompilerContext(), () =>
            {
                My<CompilerParameters>.Instance.Strict = true;

                var subject = My<DowncastPermissions>.Instance;
                Assert.IsFalse(subject.CanBeReachedByDowncast(ITypeFor<Derived>(), ITypeFor<IInterface>()));
            });
        }

        [Test]
        public void ArrayDowncastIsNotAllowed()
        {
            Environment.With(new CompilerContext(), () =>
            {
                var subject = My<DowncastPermissions>.Instance;
                Assert.IsFalse(subject.CanBeReachedByDowncast(ITypeFor<string[]>(), ITypeFor<object[]>()));
            });
        }

        private static IType ITypeFor<T>()
        {
            return My<TypeSystemServices>.Instance.Map(typeof(T));
        }
    }
}
