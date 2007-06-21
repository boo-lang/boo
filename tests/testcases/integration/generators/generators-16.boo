import NUnit.Framework

a = range(3)
g = i*2 for i in a
Assert.AreEqual("0 2 4", join(g))

a = range(3, 6)
Assert.AreEqual("6 8 10", join(g))
