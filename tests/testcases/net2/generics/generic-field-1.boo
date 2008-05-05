"""
42
"""
class Container[of T]:
	public value as T
	
c = Container[of int](value: 21)
print c.value * 2
