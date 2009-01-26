"""
first
second
"""
class Foo:
	[property(First)] _first = null
	[property(Second)] _second = null
	def constructor(value):
		First, Second = value
		
f = Foo(["first", "second"])
print f.First
print f.Second
