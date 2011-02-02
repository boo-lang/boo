"""
BCE0103-cannot-extend-array.boo(7,11): BCE0103: Cannot extend final type '(Foo)'.
"""
class Foo:
	pass
	
class Bar((Foo)):
	pass
