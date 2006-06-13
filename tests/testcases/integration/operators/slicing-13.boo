import NUnit.Framework

a = "12345"

Assert.AreEqual("123", a[-10:3])
Assert.AreEqual("345", a[-3:10])
