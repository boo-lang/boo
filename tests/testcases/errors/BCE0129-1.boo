"""
BCE0129-1.boo(10,9): BCE0129: Invalid extension definition, only static methods with at least one argument are allowed.
BCE0129-1.boo(14,9): BCE0129: Invalid extension definition, only static methods with at least one argument are allowed.
BCE0129-1.boo(17,16): BCE0129: Invalid extension definition, only static methods with at least one argument are allowed.
"""
import Boo.Lang.Compiler

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

