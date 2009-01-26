"""
class Customer(object):

	_fname as string

	_lname = null

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
	[getter(LastName)] _lname = null
	[getter(Friends as System.Collections.ICollection)] _friends as System.Collections.ArrayList

