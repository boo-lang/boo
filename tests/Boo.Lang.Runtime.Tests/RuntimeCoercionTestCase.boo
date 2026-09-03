namespace Boo.Lang.Runtime.Tests

import System
import Boo.Lang.Runtime
import NUnit.Framework

public class Coercible(ICoercible):
	public Invocations = Boo.Lang.List()

	def Coerce(to as Type) as object:
		Invocations.Add(to)
		return self

public class Integer:
	protected _value as int

	def constructor(value as int):
		_value = value

	Value as int:
		get: return _value

public static class IntegerExtensions:
	[Extension]
	def op_Implicit(i as Integer) as int:
		return i.Value

public class ImplicitInt(Integer):
	def constructor(value as int):
		super(value)

	static def op_Implicit(i as ImplicitInt) as int:
		return i._value

[TestFixture]
class RuntimeCoercionTestCase:
	[Test]
	def TestICoercible():
		c = Coercible()
		Assert.AreSame(c, Coerce(c, string))
		Assert.AreSame(c, Coerce(c, int))

		Assert.AreEqual(Boo.Lang.List((string, int)), c.Invocations)

	[Test]
	def TestNumericPromotion():
		value = 41.51
		Assert.AreEqual(value cast int, Coerce(value, int) cast int)

	[Test]
	def TestImplicitCast():
		i = ImplicitInt(42)
		Assert.AreSame(i, Coerce(i, i.GetType()))
		Assert.AreEqual(42, Coerce(i, int))

	[Test]
	def TestImplicitCastExtension():
		i = Integer(42)
		RuntimeServices.WithExtensions(IntegerExtensions):
			Assert.AreEqual(42, Coerce(i, int))

	[Test]
	def TestIdentity():
		s = "foo"
		Assert.AreSame(s, Coerce(s, string))
		Assert.AreSame(s, Coerce(s, object))

	static def Coerce(value as object, to as Type) as object:
		return RuntimeServices.Coerce(value, to)
