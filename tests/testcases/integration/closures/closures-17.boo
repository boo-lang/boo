import NUnit.Framework

def foo(o1, o2):
	c1 = { return o1 }
	c2 = { o1 = o2 }

	Assert.AreSame(o1, c1())

	c2()
	Assert.AreSame(o2, c1(), "parameters must be shared between closures")
	return c1
	
o1 = object()
o2 = object()
c = foo(o1, o2)
Assert.AreSame(o2, c())


