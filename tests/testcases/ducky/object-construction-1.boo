"""
John Cleese
Fifi
"""
class Person:
	[property(Name)] _name = ""
	
class Dog:
	[property(Name)] _name = ""
	
def New(type as System.Type, name as string):
	return type(Name: name)
	
p = New(Person, "John Cleese")
assert Person is p.GetType()
print p.Name

d = New(Dog, "Fifi")
assert Dog is d.GetType()
print d.Name


