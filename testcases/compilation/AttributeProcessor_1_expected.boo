class Customer:
	
	_fname as string
	_lname as string	
	
	def constructor(fname as string, lname as string):
		raise ArgumentNullException("fname") unless fname
		raise ArgumentNullException("lname") unless lname
		_fname = fname
		_lname = lname

