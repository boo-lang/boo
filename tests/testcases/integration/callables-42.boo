import NUnit.Framework
	
a = { return "foo" }, { return 3 }
Assert.AreEqual("foo", a[0]())
Assert.AreEqual(3, a[-1]())
