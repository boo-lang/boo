#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo B. de Oliveira
//
// Permission is hereby granted, free of charge, to any person 
// obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, 
// publish, distribute, sublicense, and/or sell copies of the Software, 
// and to permit persons to whom the Software is furnished to do so, 
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
// OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

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
			_list = new List().Add("um").Add("dois").Add("tres");
		}

		[Test]
		public void TestCount()
		{
			AssertEquals(3, _list.Count);
		}
		
		[Test]
		public void Remove()
		{
			_list.Remove("dois");
			AssertItems("um", "tres");
			_list.Remove("um");
			AssertItems("tres");
			_list.Remove("tres");
			AssertItems();
		}
		
		[Test]
		public void RemoveAt()
		{
			_list.RemoveAt(2);
			AssertItems("um", "dois");
			_list.RemoveAt(0);
			AssertItems("dois");
			_list.RemoveAt(-1);
			AssertItems();
		}
		
		[Test]
		public void Insert()
		{
			_list.Insert(-1, "foo");
			AssertItems("um", "dois", "foo", "tres");
			_list.Insert(0, "bar");
			AssertItems("bar", "um", "dois", "foo", "tres");
			_list.Insert(1, "baz");
			AssertItems("bar", "baz", "um", "dois", "foo", "tres");
		}

		[Test]
		public void TestAddUnique()
		{
			_list.AddUnique("dois");
			AssertItems("um", "dois", "tres");
		}

		[Test]
		public void TestToString()
		{
			AssertEquals("um, dois, tres", _list.ToString());
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
