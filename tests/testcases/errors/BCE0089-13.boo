"""
BCE0089-13.boo(12,5): BCE0089: Type 'Foo' already has a definition for 'bar()'.
"""
class Foo:

	def bar():
		pass
		
	def bar(value):
		pass
		
def bar(self as Foo):
	pass
