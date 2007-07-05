"""
42
"""
class Foo:
	
	[getter(Values)]
	_values = (1, 2, 3)
	
	
d as duck = Foo()
d.Values[0] = 42
print d.Values[0]
