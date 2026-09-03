namespace BooCompiler.Tests.TypeSystem.Services

import System
import Boo.Lang.Compiler
import Boo.Lang.Compiler.TypeSystem
import Boo.Lang.Compiler.TypeSystem.Services
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class DowncastPermissionsTest:
	class Base:
		pass

	class Derived(Base):
		pass

	interface IInterface:
		pass

	[Test]
	def RegularDowncastAllowedByDefault():
		RunInCompilerContextEnvironment() do:
			subject1 = My[of DowncastPermissions].Instance
			Assert.IsTrue(subject1.CanBeReachedByDowncast(ITypeFor[of Derived](), ITypeFor[of Base]()))

	[Test]
	def InterfaceDowncastAllowedByDefault():
		RunInCompilerContextEnvironment() do:
			subject = My[of DowncastPermissions].Instance
			Assert.IsTrue(subject.CanBeReachedByDowncast(ITypeFor[of Derived](), ITypeFor[of IInterface]()))

	[Test]
	def InterfaceDowncastNotAllowedInStrictMode():
		RunInCompilerContextEnvironment() do:
			My[of CompilerParameters].Instance.Strict = true

			subject = My[of DowncastPermissions].Instance
			Assert.IsFalse(subject.CanBeReachedByDowncast(ITypeFor[of Derived](), ITypeFor[of IInterface]()))

	[Test]
	def ArrayDowncastIsNotAllowed():
		RunInCompilerContextEnvironment() do:
			subject = My[of DowncastPermissions].Instance
			Assert.IsFalse(subject.CanBeReachedByDowncast(ITypeFor[of (string)](), ITypeFor[of (object)]()))

	private def RunInCompilerContextEnvironment(action as Action):
		ActiveEnvironment.With(CompilerContext().Environment):
			action()

	private static def ITypeFor[of T]() as IType:
		return My[of TypeSystemServices].Instance.Map(typeof(T))
