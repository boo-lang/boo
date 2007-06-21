import NUnit.Framework

i = 0
g = (++i)*j for j in range(3)

Assert.AreEqual("0 2 6", join(g))
Assert.AreEqual(3, i)
