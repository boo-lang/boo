"""
BCE0161-2.boo(8,21): BCE0161: Type constraint 'SomeClass' cannot be used together with the 'class' constraint on generic parameter 'T'.
"""

class SomeClass:
	pass

class C[of T(class, SomeClass)]:
	pass
