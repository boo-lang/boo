"""
class Customer:

	_fname as string

	FirstName as string:
		get:
			return _fname
		set:
			self._fname = value
			FirstNameChanged(self, System.EventArgs.Empty)

	event FirstNameChanged as System.EventHandler
"""
class Customer:
	[property(FirstName, Observable: true)]
	_fname as string
