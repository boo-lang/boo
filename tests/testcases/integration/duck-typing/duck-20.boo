struct Person:
	[property(Name)]
	_name as string
	def constructor(name):
		_name = name

p = Person("John") // static typing
d as duck = p // dynamic typing

assert d isa Person, "d isa Person"
assert p.Name == d.Name, "p.Name"

d.Name = "Eric"
assert "Eric" == d.Name
assert "John" == p.Name # we're only changing a boxed version

d._name = "John"
assert "John" == d._name
 





