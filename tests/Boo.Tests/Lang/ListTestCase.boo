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

namespace Boo.Tests

import NUnit.Framework

[TestFixture]
class ListTestCase:
	_list as List
	
	[SetUp]
	def SetUp():
		_list = ["um", "dois", "tres"]
		
	[Test]
	def TestEquals():
		Assert.IsTrue([] == [])
		Assert.IsTrue(["foo", 1] == ["foo", 1])
		Assert.IsFalse(["foo", 1] == [1, "foo"])
		Assert.IsFalse([1] == [])

	[Test]
	def Count():
		Assert.AreEqual(3, _list.Count)
	
	[Test]
	def Remove():
		_list.Remove("dois")		
		Assert.AreEqual(["um", "tres"], _list)
		
		_list.Remove("um");
		Assert.AreEqual(["tres"], _list)
		
		_list.Remove("tres")
		Assert.AreEqual([], _list)
	
	[Test]
	def RemoveAt():
		_list.RemoveAt(2)
		Assert.AreEqual(["um", "dois"], _list)
		
		_list.RemoveAt(0)
		Assert.AreEqual(["dois"], _list)
		
		_list.RemoveAt(-1)
		Assert.AreEqual([], _list)
	
	[Test]
	def Insert():
		_list.Insert(-1, "foo")
		Assert.AreEqual(["um", "dois", "foo", "tres"], _list)
		
		_list.Insert(0, "bar")
		Assert.AreEqual(["bar", "um", "dois", "foo", "tres"], _list)
		
		_list.Insert(1, "baz")
		Assert.AreEqual(["bar", "baz", "um", "dois", "foo", "tres"], _list)

	[Test]
	def AddUnique():
		_list.AddUnique("dois")
		Assert.AreEqual(["um", "dois", "tres"], _list)
		
		_list.AddUnique("quatro")
		Assert.AreEqual(["um", "dois", "tres", "quatro"], _list)

	[Test]
	def TestToString():
		Assert.AreEqual("um, dois, tres", _list.ToString())
		
	[Test]
	def Pop():
		Assert.AreEqual("tres", _list.Pop())
		Assert.AreEqual(["um", "dois"], _list)
		Assert.AreEqual("dois", _list.Pop())
		Assert.AreEqual(["um"], _list)
		Assert.AreEqual("um", _list.Pop())
		Assert.AreEqual([], _list)
		
	[Test]
	def PopIndex():
		Assert.AreEqual("dois", _list.Pop(-2))
		Assert.AreEqual(["um", "tres"], _list)
		Assert.AreEqual("um", _list.Pop(0))
		Assert.AreEqual(["tres"], _list)

