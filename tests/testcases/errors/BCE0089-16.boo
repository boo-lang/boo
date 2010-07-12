"""
BCE0089-16.boo(9,16): BCE0089: Type 'Foo' already has a definition for 'constructor()'.
"""
class Foo:

	static def constructor():
		pass
	
	static def constructor():
		pass
		
	def constructor():
		pass
