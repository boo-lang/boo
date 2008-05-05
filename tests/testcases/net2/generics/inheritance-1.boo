"""
42
"""
class Base[of T]:
	pass
	
class Derived[of T](Base[of T]):
	public value as T
	def constructor(value as T):
		self.value = value
		
print Derived[of int](42).value
		

		

