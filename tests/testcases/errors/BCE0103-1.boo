"""
BCE0103-1.boo(7,11): BCE0103: Cannot extend final type 'Foo'.
"""
final class Foo:
	pass
	
class Bar(Foo):
	pass
