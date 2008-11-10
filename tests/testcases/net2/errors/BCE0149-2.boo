"""
BCE0149-2.boo(15,35): BCE0149: The type 'C2' must derive from 'C1' in order to substitute the generic parameter 'U' in 'GenericType[of C1].GenericMethod[of U]()'.
"""

class C1:
	pass

class C2:
	pass

class GenericType[of T (C1)]:
	def GenericMethod[of U(T)]():
		print typeof(U)

GenericType[of C1]().GenericMethod[of C2]()
