namespace Boo.Lang.Tests.Environments

import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class CachingEnvironmentTest:
	[Test]
	def InstancesAreCached():
		instance = "42"
		environment = TestEnvironment(instance)
		subject = CachingEnvironment(environment)

		ActiveEnvironment.With(subject):
			Assert.AreSame(instance, My[of string].Instance)
			Assert.AreSame(instance, My[of string].Instance)

		Assert.AreEqual(1, environment.ProvideCount)

	[Test]
	def CompatibleInstancesAreReturned():
		instance = "42"
		environment = TestEnvironment(instance)
		subject = CachingEnvironment(environment)

		ActiveEnvironment.With(subject):
			Assert.AreSame(instance, My[of string].Instance)
			Assert.AreSame(instance, My[of object].Instance)

		Assert.AreEqual(1, environment.ProvideCount)
