"""
class Customer:

	_fname as string

	FirstName as string:
		get:
			return _fname
		set:
			raise System.ArgumentException('FirstName') unless (value is not null)
			_fname = value
"""
class Customer:
	[property(FirstName, value is not null)]
	_fname as string
