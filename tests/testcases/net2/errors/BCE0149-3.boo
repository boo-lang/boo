"""
BCE0149-3.boo(18,41): BCE0149: The type 'Derived2' must derive from 'Derived1' in order to substitute the generic parameter 'U' in 'GenericType[of Derived1].GenericMethod[of U](U)'.
"""

class Base:
	pass

class Derived1 (Base):
	pass

class Derived2 (Base):
	pass

class GenericType[of T (Base)]:
	def GenericMethod[of U(T)](parameter as U):
		print typeof(U)

GenericType[of Derived1]().GenericMethod(Derived2())
