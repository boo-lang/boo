import NUnit.Framework

c = __eval__(a=1, b=2, a+b)
Assert.AreEqual(1, a)
Assert.AreEqual(2, b)
Assert.AreEqual(3, c)
