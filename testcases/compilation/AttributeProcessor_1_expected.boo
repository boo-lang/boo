class Customer:
	
	_fname as string
	_lname as string	
	
	def constructor(fname as string, lname as string):
		raise System.ArgumentNullException("fname") if fname is null
		raise System.ArgumentNullException("lname") if lname is null
		_fname = fname
		_lname = lname

