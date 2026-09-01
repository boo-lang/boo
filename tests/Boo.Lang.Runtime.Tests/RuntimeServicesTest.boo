namespace Boo.Lang.Runtime.Tests

import System
import System.Globalization
import Boo.Lang.Runtime
import NUnit.Framework

[TestFixture]
class RuntimeServicesTest:
	[Test]
	def CheckNumericPromotion():
		ic = CultureInfo.InvariantCulture
		cnp = RuntimeServices.CheckNumericPromotion
		Assert.AreEqual(3, cnp(3).ToInt32(ic))
		Assert.AreEqual(1024L, cnp(1024).ToInt64(ic))

		Assert.AreEqual(true, cnp(3).ToBoolean(ic))
		Assert.AreEqual(false, cnp(0).ToBoolean(ic))

		Assert.AreEqual(0, cnp(false).ToInt32(ic))
		Assert.AreEqual(1, cnp(true).ToInt32(ic))

	[Test]
	def CheckNumericPromotionWithNull():
		AssertNumericPromotionThrows[of NullReferenceException](null)

	[Test]
	def CheckNumericPromotionWithString():
		AssertNumericPromotionThrows[of InvalidCastException]("")

	[Test]
	def CheckNumericPromotionWithDate():
		AssertNumericPromotionThrows[of InvalidCastException](DateTime.Now)

	def AssertNumericPromotionThrows[of TException](value as IConvertible):
		try:
			RuntimeServices.CheckNumericPromotion(value)
			Assert.Fail()
		except e as Exception:
			Assert.IsInstanceOf(TException, e)

	[Test]
	def RuntimeGivesUsefulMessageForMissingOperator():
		try:
			RuntimeServices.InvokeBinaryOperator("op_BitwiseAnd", "", 42)
		except x as MissingMethodException:
			Assert.AreEqual("Bitwise and is not applicable to operands 'System.String' and 'System.Int32'.", x.Message)
			return
		Assert.Fail("Exception expected")
