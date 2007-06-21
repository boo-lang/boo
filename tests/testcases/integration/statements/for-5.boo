import NUnit.Framework

values = []
for i in range(10):
	break if i > 4
	values.Add(i)
	
Assert.AreEqual([0, 1, 2, 3, 4], values)
