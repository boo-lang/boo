"""
BCE0103-2.boo(7,11): BCE0103: Cannot extend final type 'Foo'.
"""
class Foo(System.ValueType):
	pass
	
class Bar(Foo):
	pass
