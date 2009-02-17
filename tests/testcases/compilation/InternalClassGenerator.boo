"""
foo: bar
"""
internal class Node:
	_prev = self
	_next = self
	
	[getter(Key)]
	_key as object
	
	[getter(Value)]
	_value as object
	
	def constructor(key, value):
		_key = key
		_value = value
		
	def Insert(item as Node):
		_next._prev = item
		item._next = _next
		item._prev = self
		_next = item

	def Generator(): //cycle through the items
		item = _next
		while item._key: #iterate until head
			yield item
			item = item._next
			
head = Node(null, null)
head.Insert(Node("foo", "bar"))
for item in head.Generator():
	print "${item.Key}: ${item.Value}"
