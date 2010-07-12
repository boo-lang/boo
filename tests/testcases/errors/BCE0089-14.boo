"""
BCE0089-14.boo(9,16): BCE0089: Type 'Foo' already has a definition for 'bar()'.
"""
class Foo:

	def bar():
		pass
	
	static def bar():
		pass
		
	def bar(value):
		pass
