"""
2 2
3 3
"""
class Provider:

	[getter(Items)]
	_items = ((1, 2), (3, 4))
	
	_current = -1
	
	def Next():
		return _items[++_current % 2]
			
p = Provider()
++p.Next()[0]
--p.Next()[1]
print join(p.Items[0])
print join(p.Items[1])
