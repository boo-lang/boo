"""
BCE0089-9.boo(9,9): BCE0089: Type 'Foo' already has a definition for 'bar()'.
"""
class Foo:

	def bar():
		pass
	
	def bar():
		pass
		
	def bar(value):
		pass
