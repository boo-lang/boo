"""
BCE0103-3.boo(7,11): BCE0103: Cannot extend final type 'Foo'.
"""
enum Foo:
	None
	
class Bar(Foo):
	pass
