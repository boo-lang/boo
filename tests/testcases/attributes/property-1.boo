"""
class Customer:

	_fname as string

	FirstName as string:
		get:
			return _fname
		set:
			_fname = value
"""
class Customer:
	[property(FirstName)] _fname as string
