"""
BCE0155-1.boo(7,14): BCE0155: Cannot create an instance of the variable type 'Foo[of T]`1.T' because it doesn't have a default constructor constraint.
"""

class Foo[of T]:
	def Bar(ref i as T):
		i = T()
