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
		[ExpectedException(typeof(NullReferenceException))]
		public void CheckNumericPromotionWithNull()
		{
			((Func<object, IConvertible>) RuntimeServices.CheckNumericPromotion)(null);
		}

		[Test]
		[ExpectedException(typeof(InvalidCastException))]
		public void CheckNumericPromotionWithString()
		{
			((Func<object, IConvertible>) RuntimeServices.CheckNumericPromotion)("");
		}

		[Test]
		[ExpectedException(typeof(InvalidCastException))]
		public void CheckNumericPromotionWithDate()
		{
			((Func<object, IConvertible>) RuntimeServices.CheckNumericPromotion)(DateTime.Now);
		}
	}
}
