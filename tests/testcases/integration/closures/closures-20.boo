import NUnit.Framework

closures = []
for i in range(3):
	closures.Add({ return i })
	
for expected, closure as callable in zip(range(3), closures):
	Assert.AreEqual(expected, closure(), "for variables are not shareable")
