import NUnit.Framework

def foo():
	return i*2 for i in range(5)

current = 0
for i in foo():
	Assert.IsTrue(current == i/2, "integer division")
	++current
