import NUnit.Framework

Assert.AreEqual("0 1 2", join(range(3)))
Assert.AreEqual("0, 1, 2", join(range(3), ", "))
Assert.AreEqual("0:1:2", join(range(3), ":"))
Assert.AreEqual("0:1:2", join(range(3), ":"[0]))
