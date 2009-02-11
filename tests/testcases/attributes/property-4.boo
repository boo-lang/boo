"""
class Customer(object):

	_fname as string

	public FirstName as string:
		get:
			return _fname
		set:
			raise System.ArgumentNullException('value') if (value is null)
			self._fname = value

"""
class Customer:
	[property(FirstName, Attributes:[Required])]
	_fname as string
