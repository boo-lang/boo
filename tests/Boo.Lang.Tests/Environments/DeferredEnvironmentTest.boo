namespace Boo.Lang.Tests.Environments

import System
import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class DeferredEnvironmentTest:
	class Foo:
		pass

	class ImprovedFoo(Foo):
		pass

	class Bar:
		pass

	[Test]
	def ExactTypeRequestIsFullfilled():
		environment = DeferredEnvironment()
		environment.Add(Foo, { Foo() })
		ActiveEnvironment.With(environment):
			Assert.IsNotNull(My[of Foo].Instance)

	[Test]
	def CompatibleTypeRequestIsFullfilled():
		environment = DeferredEnvironment()
		environment.Add(ImprovedFoo, { ImprovedFoo() })
		ActiveEnvironment.With(environment):
			Assert.IsNotNull(My[of Foo].Instance)

	[Test]
	def IncompatibleTypeRequestIsNotFullfilled():
		environment = DeferredEnvironment()
		environment.Add(Foo, { Foo() })
		ActiveEnvironment.With(environment):
			try:
				My[of Bar].Instance.ToString()
			except as InvalidOperationException:
				return
			Assert.Fail("InvalidOperationException expected")
