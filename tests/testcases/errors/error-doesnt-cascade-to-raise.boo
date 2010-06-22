"""
error-doesnt-cascade-to-raise.boo(7,11): BCE0019: 'Bar' is not a member of 'Foo'.
"""
class Foo:
	pass
	
o = Foo().Bar()
raise o

