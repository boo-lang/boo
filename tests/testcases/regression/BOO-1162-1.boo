"""
OK
"""

class SomeClass:
	pass
	
struct SomeStruct:
	inHomageToTheVerifier as int

struct SomeOtherStruct:
	inHomageToTheVerifier as int

def ResolvableNullAmbiguity(x as SomeStruct):
	pass
def ResolvableNullAmbiguity(x as SomeClass):
	print "OK"
def ResolvableNullAmbiguity(x as SomeOtherStruct):
	pass


ResolvableNullAmbiguity(null)

