"""
class Customer:

	_fname as string

	_lname

	LastName:
		get:
			return _lname

	FirstName as string:
		get:
			return _fname
"""
class Customer:
	[getter(FirstName)] _fname as string
	[getter(LastName)] _lname
