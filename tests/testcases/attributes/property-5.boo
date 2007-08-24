"""
class Customer:

	_fname as string

	protected FirstName as string:
		get:
			return _fname
		set:
			self._fname = value
"""
class Customer:
	[property(FirstName, Protected: true)] _fname as string
