"""
homer

"""
class Person:
	
	[getter(FirstName)]
	_fname as string
	
	def constructor(fname as string):
		_fname = fname
		
homer = Person("homer")
print(homer.FirstName)
