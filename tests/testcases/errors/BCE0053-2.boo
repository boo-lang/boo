"""
BCE0053-2.boo(15,1): BCE0053: Property 'Person.Name' is read only.
"""
class Person:
	_name as string
	
	def constructor(name as string):
		_name = name
		
	Name:
		get:
			return _name
			
p = Person("Homer")
p.Name = "Simpson"
