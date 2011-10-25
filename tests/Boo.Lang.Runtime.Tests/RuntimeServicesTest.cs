using System;
using System.Globalization;
using NUnit.Framework;

namespace Boo.Lang.Runtime.Tests
{
	[TestFixture]
	public class RuntimeServicesTest
	{
		[Test]
		public void CheckNumericPromotion()
		{
			var ic = CultureInfo.InvariantCulture;
			Func<object, IConvertible> cnp = RuntimeServices.CheckNumericPromotion;
			Assert.AreEqual(3, cnp(3).ToInt32(ic));
			Assert.AreEqual(1024L, cnp(1024).ToInt64(ic));

			Assert.AreEqual(true, cnp(3).ToBoolean(ic));
			Assert.AreEqual(false, cnp(0).ToBoolean(ic));

			Assert.AreEqual(0, cnp(false).ToInt32(ic));
			Assert.AreEqual(1, cnp(true).ToInt32(ic));
		}

		[Test]
		public void CheckNumericPromotionWithNull()
		{
			AssertNumericPromotionThrows<NullReferenceException>(null);
		}

		[Test]
		public void CheckNumericPromotionWithString()
		{
			AssertNumericPromotionThrows<InvalidCastException>("");
		}

		[Test]
		public void CheckNumericPromotionWithDate()
		{
			AssertNumericPromotionThrows<InvalidCastException>(DateTime.Now);
		}

		private void AssertNumericPromotionThrows<TException>(IConvertible value)
		{
			try
			{
				RuntimeServices.CheckNumericPromotion(value);
				Assert.Fail();
			}
			catch (Exception e)
			{
				Assert.IsInstanceOfType(typeof(TException), e);
			}
		}

		[Test]
		public void RuntimeGivesUsefulMessageForMissingOperator()
		{
			try
			{
				RuntimeServices.InvokeBinaryOperator("op_BitwiseAnd", "", 42);
			}
			catch (MissingMethodException x)
			{
				Assert.AreEqual("Bitwise and is not applicable to operands 'System.String' and 'System.Int32'.", x.Message);
				return;
			}
			Assert.Fail("Exception expected");
		}
	}
}
