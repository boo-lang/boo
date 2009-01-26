"""
third
first
second
"""
class Foo:
	[property(First)] _first = null
	[property(Second)] _second = null
	def constructor(value):
		First, _second, third = value
		print third
		
f = Foo(["first", "second", "third"])
print f.First
print f.Second
