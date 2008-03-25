"""
BCE0157-1.boo(8,5): BCE0157: Generic types without all generic parameters defined cannot be instantiated.
"""

class AClass[of T]:
	pass

c = AClass()

print c