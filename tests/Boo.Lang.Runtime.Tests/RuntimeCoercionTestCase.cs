using System;
using NUnit.Framework;

namespace Boo.Lang.Runtime.Tests
{
	class Coercible : ICoercible
	{
		public List Invocations = new List();

		public object Coerce(Type to)
		{
			Invocations.Add(to);
			return this;
		}
	}

	internal class Integer
	{
		protected int _value;

		public Integer(int value)
		{
			_value = value;
		}

		public int Value
		{
			get { return _value;  }
		}
	}

	class IntegerExtensions
	{
		[Boo.Lang.Extension]
		public static int op_Implicit(Integer i)
		{
			return i.Value;
		}
	}

	class ImplicitInt : Integer
	{
		public ImplicitInt(int value) : base(value)
		{
		}

		public static implicit operator int(ImplicitInt i)
		{
			return i._value;
		}
	}

	[TestFixture]
	public class RuntimeCoercionTestCase
	{
		[Test]
		public void TestICoercible()
		{
			Coercible c = new Coercible();
			Assert.AreSame(c, Coerce(c, typeof (string)));
			Assert.AreSame(c, Coerce(c, typeof (int)));

			Assert.AreEqual(new List(new object[] { typeof(string), typeof(int) }), c.Invocations);
		}

		[Test]
		public void TestNumericPromotion()
		{	
			Assert.AreEqual(42, Coerce(41.51, typeof(int)));
		}

		[Test]
		public void TestImplicitCast()
		{	
			ImplicitInt i = new ImplicitInt(42);
			Assert.AreSame(i, Coerce(i, i.GetType()));
			Assert.AreEqual(42, Coerce(i, typeof(int)));
		}

		[Test]
		public void TestImplicitCastExtension()
		{	
			Integer i = new Integer(42);
			RuntimeServices.WithExtensions(typeof (IntegerExtensions), delegate
           	{
           		Assert.AreEqual(42, Coerce(i, typeof (int)));
           	});
		}

		[Test]
		public void TestIdentity()
		{
			string s = "foo";
			Assert.AreSame(s, Coerce(s, typeof(string)));
			Assert.AreSame(s, Coerce(s, typeof(object)));
		}

		static object Coerce(object value, Type to)
		{
			return RuntimeServices.Coerce(value, to);
		}
	}
}
