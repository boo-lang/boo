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
