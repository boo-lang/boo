import System

def NullableTest():
	return Default(Nullable[of int])

var value = NullableTest()
assert not value.HasValue