"""
BCE0024-2.boo(9,9): BCE0024: The type 'Foo' does not have a visible constructor that matches the argument list '()'.
"""
class Foo:
	def constructor(item):
		pass
		
class Bar(Foo):
	def constructor():
		pass
