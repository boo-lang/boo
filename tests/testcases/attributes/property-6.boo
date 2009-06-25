"""
class Customer(object):

	_friends as System.Collections.ArrayList

	protected Friends as System.Collections.ICollection:
		get:
			return _friends
		set:
			self._friends = value
			FriendsChanged(self, System.EventArgs.Empty)

	protected event FriendsChanged as System.EventHandler
"""
class Customer:
	[property(Friends as System.Collections.ICollection, Protected: true, Observable: true)] _friends as System.Collections.ArrayList
