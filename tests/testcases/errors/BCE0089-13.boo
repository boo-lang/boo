"""
BCE0089-13.boo(13,5): BCE0089: Type 'Foo' already has a definition for 'bar()'.
"""
class Foo:

	def bar():
		pass
		
	def bar(value):
		pass
		
[Extension]
def bar(f as Foo):
	pass
