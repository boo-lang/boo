using System;
using NUnit.Framework;
using Boo.Lang;

namespace Boo.Tests.Lang
{
	/// <summary>	
	/// </summary>
	[TestFixture]
	public class ListTestCase : Assertion
	{
		List _list;

		[SetUp]
		public void SetUp()
		{
			_list = new List("um", "dois", "três");
		}

		[Test]
		public void TestCount()
		{
			AssertEquals(3, _list.Count);
		}

		[Test]
		public void TestAddUnique()
		{
			_list.AddUnique("dois");
			AssertItems("um", "dois", "três");
		}

		[Test]
		public void TestToString()
		{
			AssertEquals("um, dois, três", _list.ToString());
		}

		void AssertItems(params object[] items)
		{			
			AssertEquals("Count", items.Length, _list.Count);
			for (int i=0; i<items.Length; ++i)
			{
				AssertEquals("[" + i + "]", items[i], _list[i]);
			}
		}
	}
}
