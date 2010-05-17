"""
1, 2, 3
"""
class A:
	[getter(Foo)]
	_foo = map([1, 2, 3]) do (item):
		return item.ToString()

def map(items, mapper as callable):
	for item in items:
		yield mapper(item)
		
print join(A().Foo, ", ")
