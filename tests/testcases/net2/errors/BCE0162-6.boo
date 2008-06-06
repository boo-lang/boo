"""
BCE0162-6.boo(5,14): BCE0162: Type 'System.Delegate' must be an interface type or a non-final class type to be used as a type constraint on generic parameter 'T'.
"""

class C[of T(System.Delegate)]:
	pass
