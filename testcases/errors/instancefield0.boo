"""
instancefield0.boo(7,14): BCE0020: 'Person.Name' can't be used without an instance. 
"""
class Person:
	public Name as string
	
print(Person.Name)
