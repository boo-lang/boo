"""
BCE0161-1.boo(8,22): BCE0161: Type constraint 'SomeClass' cannot be used together with the 'struct' constraint on generic parameter 'T'.
"""

class SomeClass:
	pass

class C[of T(struct, SomeClass)]:
	pass

