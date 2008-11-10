"""
BCE0149-1.boo(11,12): BCE0149: The type 'int' must derive from 'Base' in order to substitute the generic parameter 'T' in 'GenericType[of T]'.
"""

class Base:
	pass

class GenericType[of T (Base)]:
	pass

GenericType[of int]()
