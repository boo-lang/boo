"""
BCE0053-3.boo(10,9): BCE0053: Property 'Person.FirstName' is read only.
BCE0053-3.boo(10,20): BCE0053: Property 'Person.LastName' is read only.
"""
class Person:
	[getter(FirstName)] _fname as string
	[getter(LastName)] _lname as string	
	
	def constructor():
		FirstName, LastName = "Homer", "Simpson"
	
p = Person()
