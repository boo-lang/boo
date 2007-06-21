"""
BCE0129-1.boo(7,9): BCE0129: Invalid extension definition, only static methods are allowed.
BCE0129-1.boo(11,9): BCE0129: Invalid extension definition, only static methods are allowed.
"""
class Foo:
	[Extension]
	def bar(s as string):
		pass
		
	[Extension]
	def constructor(s as string):
		pass
