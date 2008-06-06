"""
BCE0162-4.boo(5,14): BCE0162: Type 'object' must be an interface type or a non-final class type to be used as a type constraint on generic parameter 'T'.
"""

class C[of T(System.Object)]:
	pass
