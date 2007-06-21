"""
third
first
second
"""
class Foo:
	[property(First)] _first
	[property(Second)] _second
	def constructor(value):
		First, _second, third = value
		print third
		
f = Foo(["first", "second", "third"])
print f.First
print f.Second
