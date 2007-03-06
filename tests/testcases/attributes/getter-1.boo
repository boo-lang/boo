"""
class Customer:

	_fname as string

	_lname

	FirstName as string:
		get:
			return _fname

	LastName:
		get:
			return _lname
"""
class Customer:
	[getter(FirstName)] _fname as string
	[getter(LastName)] _lname
