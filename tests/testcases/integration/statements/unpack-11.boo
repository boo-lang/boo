"""
first
second
"""
class Foo:
	[property(First)] _first
	[property(Second)] _second
	def constructor(value):
		First, Second = value
		
f = Foo(["first", "second"])
print f.First
print f.Second
