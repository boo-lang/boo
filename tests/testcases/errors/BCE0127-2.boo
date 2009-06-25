"""
BCE0127-2.boo(16,13): BCE0127: A ref or out argument must be an lvalue: 'self'
BCE0127-2.boo(16,12): BCE0017: The best overload for the method 'Bar.Ref(Bar)' is not compatible with the argument list '(Bar)'.
"""

struct Foo:
	def Ref(ref x as Foo):
		pass
	def SelfRef():
		Ref(self) #ok since self is a struct/value type

class Bar:
	def Ref(ref x as Bar):
		pass
	def SelfRef():
		Ref(self) #! (self is not a value type)

