"""
class Customer(object):

	_fname as string

	public FirstName as string:
		get:
			return _fname
		set:
			self._fname = value
			FirstNameChanged(self, System.EventArgs.Empty)

	public event FirstNameChanged as System.EventHandler
"""
class Customer:
	[property(FirstName, Observable: true)]
	_fname as string
