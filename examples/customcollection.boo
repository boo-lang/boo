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
