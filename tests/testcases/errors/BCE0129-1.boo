"""
BCE0129-1.boo(8,9): BCE0129: Invalid extension definition, only static methods with at least one argument are allowed.
BCE0129-1.boo(12,9): BCE0129: Invalid extension definition, only static methods with at least one argument are allowed.
BCE0129-1.boo(15,16): BCE0129: Invalid extension definition, only static methods with at least one argument are allowed.
"""
class Foo:
	[Extension]
	def bar(s as string):
		pass
		
	[Extension]
	def constructor(s as string):
		pass
	[Extension]
	static def Ext():
		pass

