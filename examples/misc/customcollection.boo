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

import System
import System.Reflection
import System.Collections

class Foo:
	[getter(Name)]
	_name as string
	
	def constructor(name as string):
		_name = name

[DefaultMember("Item")]
[EnumeratorItemType(Foo)]
class FooCollection(ICollection):
	
	_items = []
	
	SyncRoot:
		get:
			return _items.SyncRoot
	
	Count:
		get:
			return len(_items)
	
	IsSynchronized:
		get:
			return _items.IsSynchronized
			
	def CopyTo(target as Array, index as int):
		_items.CopyTo(target, index)
		
	def GetEnumerator():
		return _items.GetEnumerator()
		
	def Add([required] item as Foo):
		_items.Add(item)
		
	Item(index as int) as Foo:
		get:
			return _items[index]

			
c = FooCollection()
c.Add(Foo("Homer"))
c.Add(Foo("Eric"))
for foo in c:
	print(foo.Name)
