"""
A module can have a docstring.
"""
namespace Foo.Bar
"""
And so can the namespace declaration.
"""

class Person:
"""
A class can have it.
With multiple lines.
"""
	_fname as string
	"""Fields can have one."""
	
	def constructor([required] fname as string):
	"""
	And so can a method or constructor.
	"""
		_fname = fname
		
	FirstName as string:
	"""And why couldn't a property?"""
		get:
			return _fname

interface ICustomer:
"""an interface."""

	def Initialize()
	"""interface method"""
	
	Name as string:
	"""interface property"""
		get
		
	event Foo as EventHandler
	"""events, yes, why not?"""

enum AnEnum:
"""and so can an enum"""
	AnItem
	"""and its items"""
	AnotherItem


