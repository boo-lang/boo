"""
Eric
Terry
John
"""
class Person:
	
	[getter(Name)]
	_name as string
	
	def constructor(name as string):
		_name = name

for p as Person in map(Person, ["Eric", "Terry", "John"]):
	print(p.Name)

