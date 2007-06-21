import NUnit.Framework

a, b = 1, 2 if false
c, d = 3, 4 if true

Assert.AreEqual(0, a)
Assert.AreEqual(0, b)
Assert.AreEqual(3, c)
Assert.AreEqual(4, d)
