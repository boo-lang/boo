import NUnit.Framework

a = false or 5
b = true or 3

Assert.AreSame(int, a.GetType())
Assert.AreSame(int, b.GetType())
Assert.AreEqual(5, a*b)
Assert.AreEqual(5, a)
Assert.AreEqual(1, b)
Assert.AreEqual(0, false and 3)

