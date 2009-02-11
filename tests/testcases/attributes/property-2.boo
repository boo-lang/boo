"""
class Customer(object):

	_fname as string

	public FirstName as string:
		get:
			return _fname
		set:
			raise System.ArgumentException('precondition \'(value is not null)\' failed:') unless (value is not null)
			self._fname = value
"""
class Customer:
	[property(FirstName, value is not null)]
	_fname as string
