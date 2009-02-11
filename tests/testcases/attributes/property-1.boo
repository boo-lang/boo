"""
class Customer(object):

	_fname as string

	public FirstName as string:
		get:
			return _fname
		set:
			self._fname = value
"""
class Customer:
	[property(FirstName)] _fname as string
