import NUnit.Framework

g = i*2 for i in range(5)

current = 0
for i in g:
	Assert.IsTrue(current == i/2, "integer division")
	++current
