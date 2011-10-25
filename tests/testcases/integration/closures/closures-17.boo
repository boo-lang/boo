
def foo(o1, o2):
	c1 = { return o1 }
	c2 = { o1 = o2 }

	assert o1 is c1()

	c2()
	assert o2 is c1(), "parameters must be shared between closures"
	return c1
	
o1 = object()
o2 = object()
c = foo(o1, o2)
assert o2 is c()


