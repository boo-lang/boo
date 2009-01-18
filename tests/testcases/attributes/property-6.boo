"""
class Customer(object):

	_friends as System.Collections.ArrayList

	protected Friends as System.Collections.ICollection:
		get:
			return _friends
		set:
			self._friends = value
"""
class Customer:
	[property(Friends as System.Collections.ICollection, Protected: true)] _friends as System.Collections.ArrayList
