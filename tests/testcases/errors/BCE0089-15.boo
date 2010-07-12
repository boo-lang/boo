"""
BCE0089-15.boo(12,9): BCE0089: Type 'Foo' already has a definition for 'constructor()'.
"""
class Foo:

	def constructor():
		pass
	
	static def constructor():
		pass
		
	def constructor():
		pass
