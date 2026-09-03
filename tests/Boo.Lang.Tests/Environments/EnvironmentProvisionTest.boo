namespace Boo.Lang.Tests.Environments

import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class EnvironmentProvisionTest:
	class Foo:
		pass

	[Test]
	def ProvisioningHappensOnDemandAndOnlyOnce():
		environment = TestEnvironment(Foo())
		provision = EnvironmentProvision[of Foo]()

		ActiveEnvironment.With(environment):
			first = provision.Instance
			second = provision.Instance
			Assert.IsNotNull(first)
			Assert.AreSame(first, second)

		Assert.AreEqual(1, environment.ProvideCount)
