"""
BCE0020-1.boo(10,15): BCE0020: An instance of type 'Foo' is required to access non static member 'bar'.
BCE0020-1.boo(15,5): BCE0020: An instance of type 'Foo' is required to access non static member 'baz'.
"""
class Foo:

	bar = 0
	
	static def go():
		print bar
		
	def baz():
		print bar
		
Foo.baz()
