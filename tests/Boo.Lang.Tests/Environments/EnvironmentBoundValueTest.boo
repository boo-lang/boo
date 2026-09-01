namespace Boo.Lang.Tests.Environments

import Boo.Lang.Environments
import NUnit.Framework

[TestFixture]
class EnvironmentBoundValueTest:
	[Test]
	def SelectRunsInsideOriginalEnvironment():
		environment = ClosedEnvironment("42")
		v as EnvironmentBoundValue[of string]

		ActiveEnvironment.With(environment):
			v = EnvironmentBoundValue.Capture[of string]()

		valueEnvironmentPair = v.Select[of (object)]({ value | array((value, ActiveEnvironment.Instance)) }).Value
		Assert.AreEqual("42", valueEnvironmentPair[0])
		Assert.AreSame(environment, valueEnvironmentPair[1])
