#region license
// boo - an extensible programming language for the CLI
// Copyright (C) 2004 Rodrigo Barreto de Oliveira
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
// 
// Contact Information
//
// mailto:rbo@acm.org
#endregion

namespace Boo.Lang.Tests

import System
import NUnit.Framework

[TestFixture]
class ListTestCase:
	_list as List
	
	[SetUp]
	def SetUp():
		_list = ["um", "dois", "tres"]
		
	[Test]
	def Slicing():
		Assert.AreEqual(["um"], _list[:1])
		Assert.AreEqual(["um", "dois"], _list[:2])
		Assert.AreEqual(["dois"], _list[1:2])
		Assert.AreEqual(["tres"], _list[-1:])
		Assert.AreEqual(["dois", "tres"], _list[-2:])
		Assert.AreEqual(["dois"], _list[-2:-1])
		Assert.AreEqual(["um", "dois", "tres"], _list[:])
		
	[Test]
	def OutOfRangeSlicing():		
		Assert.AreEqual(["um", "dois", "tres"], _list[:10])		
		Assert.AreEqual([], _list[10:])		
		Assert.AreEqual(["tres"], _list[2:10])
		
		Assert.AreEqual(["um", "dois", "tres"], _list[-4:])
		Assert.AreEqual(["um"], _list[-4:-2])
		Assert.AreEqual(["um", "dois", "tres"], _list[-10:10])
		
	[Test]
	def Indexing():
		Assert.AreEqual("um", _list[0])
		Assert.AreEqual("um", _list[-3])
		Assert.AreEqual("dois", _list[1])
		Assert.AreEqual("dois", _list[-2])
		Assert.AreEqual("tres", _list[2])
		Assert.AreEqual("tres", _list[-1])
		
	[Test]
	[ExpectedException(IndexOutOfRangeException)]
	def NegativeIndexOutOfRange():
		Assert.IsNull(_list[-4])
		
	[Test]
	[ExpectedException(IndexOutOfRangeException)]
	def PositiveIndexOutOfRange():
		Assert.IsNull(_list[3])
		
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
		
		obj1 = object()
		obj2 = object()
		
		l = []
		l.AddUnique(obj1)
		l.AddUnique(obj1)
		l.AddUnique(obj2)
		l.AddUnique(obj1)
		l.AddUnique(obj2)
		
		Assert.AreEqual([obj1, obj2], l)

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

