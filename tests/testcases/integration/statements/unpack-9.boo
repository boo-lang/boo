"""
first
second
"""
class Foo:
	public first as object
	public second as object
	def constructor(value):
		first, second = value
		
f = Foo(["first", "second"])
print f.first
print f.second
