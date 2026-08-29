"""
homer

"""
class Person:
	
	_fname as string
	
	def constructor(fname as string):
		_fname = fname
		
	FirstName:
		get:
			return _fname
		
homer = Person("homer")
print(homer.FirstName)
