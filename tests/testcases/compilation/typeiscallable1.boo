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
		
	static def create(name):
		return Person(name)

for p as Person in map(Person.create, ["Eric", "Terry", "John"]):
	print(p.Name)

