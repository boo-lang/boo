"""
Derived
"""

class Base:
	pass

class Derived (Base):
	pass

class GenericType[of T]:
	def GenericMethod[of U(T)](parameter as U):
		print typeof(U)

GenericType[of Base]().GenericMethod(Derived())
