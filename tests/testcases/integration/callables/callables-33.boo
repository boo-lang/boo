import System


callable Function(item) as object

def identity(item):
	return item
	
d as MulticastDelegate = cast(Function, identity)
f as Function = d
assert "foo" == f("foo")
