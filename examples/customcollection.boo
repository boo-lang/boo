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
	
	SyncRoot as object:
		get:
			return _items.SyncRoot
	
	Count as int:
		get:
			return len(_items)
	
	IsSynchronized as bool:
		get:
			return _items.IsSynchronized
			
	def CopyTo(array as Array, count as int):
		_items.CopyTo(array, count)
		
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
