"""
BCE0073-1.boo(5,18): BCE0073: Abstract method 'Foo.Bar' cannot have a body.
"""
class Foo:
	abstract def Bar():
		print("shouldn't be here!")
