"""
homer simpson

"""
class Person:
	
	[getter(FirstName)]
	_fname as string
	
	[getter(LastName)]
	_lname as string
	
	def constructor(fname as string, lname as string):
		_fname = fname
		_lname = lname
		
	def ToString():
		return "${_fname} ${_lname}"
		
homer = Person("homer", "simpson")
print(homer.ToString())
