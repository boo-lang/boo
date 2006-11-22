class Person:
	[property(Name)]
	_name as string
	def constructor(name):
		_name = name

p = Person("John") // static typing
d as duck = p // dynamic typing

assert p is d, "being duck does not change the reference"
assert d isa Person, "d isa Person"
assert p.Name == d.Name, "p.Name"

d.Name = "Eric"
assert "Eric" == p.Name
assert p.Name is d.Name





