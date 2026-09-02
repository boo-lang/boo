namespace BooCompiler.Tests.Util

import System.Collections.Generic
import Boo.Lang.Compiler.Util
import NUnit.Framework

[TestFixture]
class ArrayEqualityComparerTest:
	[Test]
	def StringEquals():
		comparer as IEqualityComparer[of (string)] = ArrayEqualityComparer[of string].Default

		a1 = ("foo", "bar")
		a2 = (string("foo".ToCharArray()), "bar")
		Assert.IsTrue(comparer.Equals(a1, a2))

	[Test]
	def ByteEquals():
		comparer as IEqualityComparer[of (byte)] = ValueTypeArrayEqualityComparer[of byte].Default

		a1 = array(byte, (0, 42))
		a2 = array(byte, (0, 42))
		Assert.IsTrue(comparer.Equals(a1, a2))
