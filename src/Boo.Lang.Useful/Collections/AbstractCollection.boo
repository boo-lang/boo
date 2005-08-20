namespace Boo.Lang.Useful.Collections

import System.Collections

class AbstractCollection(ICollection):
	
	_items = []
	
	protected InnerList:
		get:
			return _items
	
	IsSynchronized:
		get:
			return _items.IsSynchronized
			
	SyncRoot:
		get:
			return _items.SyncRoot;
			
	Count:
		get:
			return _items.Count
			
	def CopyTo(targetArray as System.Array, index as int):
		_items.CopyTo(targetArray, index)
		
	def GetEnumerator():
		return _items.GetEnumerator()
			

	
	