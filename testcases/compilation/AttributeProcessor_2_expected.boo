class Customer:
	
	_fname as string
	_lname as string
	
	def constructor(fname as string, lname as string):
		raise System.ArgumentNullException("fname") unless fname
		raise System.ArgumentNullException("lname") unless lname
		_fname = fname
		_lname = lname
				
	LastName as string:
		get:
			return _lname
	
	FirstName as string:
		get:
			return _fname
