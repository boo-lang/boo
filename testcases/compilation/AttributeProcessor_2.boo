class Customer:	
	
	[getter(FirstName)]
	_fname as string
	
	[getter(LastName)]
	_lname as string
	
	def constructor([required] fname as string, [required] lname as string):
		_fname = fname
		_lname = lname

