"""
BCE0141.boo(6,26): BCE0141: Duplicate parameter name 'dup' in 'Foo.constructor(object, object)'.
BCE0141.boo(9,24): BCE0141: Duplicate parameter name 'dup' in 'BCE0141Module.method(int, string)'.
"""
class Foo:
	def constructor(dup, dup):
		pass
		
def method(dup as int, dup as string):
	print dup, dup
	
method(1, "snaffu!")
