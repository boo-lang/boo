#region license
// Copyright (c) 2004, Rodrigo B. de Oliveira (rbo@acm.org)
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//     * Neither the name of Rodrigo B. de Oliveira nor the names of its
//     contributors may be used to endorse or promote products derived from this
//     software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

namespace Boo.Tests.Lang

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
		Assert.AreEqual("[um, dois, tres]", _list.ToString())
		
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

