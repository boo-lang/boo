using System.Collections

mixin Collection<T>(ICollection, IEnumerable):
	
	_items = ArrayList()
	
	def Add(item as T):
		_items.Add(item)
		
	Count:
		get:
			return _items.Count
			
class CustomerCollection:

	include Collection<Customer>
