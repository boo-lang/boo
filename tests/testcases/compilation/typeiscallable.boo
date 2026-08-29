"""
Homer
"""
class Person:

	[getter(Name)]
	_name as string
	
	def constructor(name as string):
		_name = name


type = Person
p as Person = type("Homer")
print(p.Name)
	
