"""
BCE0004-3.boo(23,30): BCE0004: Ambiguous reference 'UnresolvableNullAmbiguity': Foo.UnresolvableNullAmbiguity(DerivedInternalClass), Foo.UnresolvableNullAmbiguity(InternalClass).
BCE0004-3.boo(24,38): BCE0004: Ambiguous reference 'UnresolvableNullAmbiguityExternal': Foo.UnresolvableNullAmbiguityExternal(System.ArgumentException), Foo.UnresolvableNullAmbiguityExternal(System.Exception).
"""

class InternalClass:
	pass
class DerivedInternalClass (InternalClass):
	pass

class Foo:
	def UnresolvableNullAmbiguity(x as InternalClass):
		pass
	def UnresolvableNullAmbiguity(x as DerivedInternalClass):
		pass

	def UnresolvableNullAmbiguityExternal(x as System.Exception):
		pass
	def UnresolvableNullAmbiguityExternal(x as System.ArgumentException):
		pass


Foo.UnresolvableNullAmbiguity(null) #!
Foo.UnresolvableNullAmbiguityExternal(null) #!

