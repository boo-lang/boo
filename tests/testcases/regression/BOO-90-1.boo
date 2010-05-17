import NUnit.Framework

class MyHash(Hash):
	pass
	
h = MyHash()
h[3] = 4
Assert.AreEqual(4, h[3])
