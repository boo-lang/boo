"""
BCE0104-1.boo(8,19): BCE0104: 'transient' can only be applied to class, field and event definitions.
BCE0104-1.boo(11,24): BCE0104: 'transient' can only be applied to class, field and event definitions.
BCE0104-1.boo(13,25): BCE0104: 'transient' can only be applied to class, field and event definitions.
"""
class Foo:

	transient def Bar():
		pass
		
	transient callable Baz()
	
	transient interface ISpam:
		pass
