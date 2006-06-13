import NUnit.Framework

value = 3
c = def:
	Assert.AreEqual(3, value)
	value = 4 # change our local copy
	Assert.AreEqual(4, value)
	
c()
Assert.AreEqual(4, value, "local variables must be shared")
