"""
BCE0089-10.boo(9,9): BCE0089: Type 'Foo' already has a definition for 'constructor(System.Int32)'.
"""
class Foo:

	def constructor(item as int):
		pass
	
	def constructor(item as int):
		pass
		
	def constructor(value):
		pass
