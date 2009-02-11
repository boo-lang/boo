"""
class Customer(object):

	_fname as string

	_lname = null

	_friends as System.Collections.ArrayList

	public FirstName as string:
		get:
			return _fname

	public LastName:
		get:
			return _lname

	public Friends as System.Collections.ICollection:
		get:
			return _friends
"""
class Customer:
	[getter(FirstName)] _fname as string
	[getter(LastName)] _lname = null
	[getter(Friends as System.Collections.ICollection)] _friends as System.Collections.ArrayList

