"""
OK
"""

class SomeClass:
	pass
struct SomeStruct:
	pass
struct SomeOtherStruct:
	pass

def ResolvableNullAmbiguity(x as SomeStruct):
	pass
def ResolvableNullAmbiguity(x as SomeClass):
	print "OK"
def ResolvableNullAmbiguity(x as SomeOtherStruct):
	pass


ResolvableNullAmbiguity(null)

