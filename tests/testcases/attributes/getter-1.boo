"""
class Customer:

	_fname as string

	_lname

	_friends as System.Collections.ArrayList

	FirstName as string:
		get:
			return _fname

	LastName:
		get:
			return _lname

	Friends as System.Collections.ICollection:
		get:
			return _friends
"""
class Customer:
	[getter(FirstName)] _fname as string
	[getter(LastName)] _lname
	[getter(Friends as System.Collections.ICollection)] _friends as System.Collections.ArrayList

