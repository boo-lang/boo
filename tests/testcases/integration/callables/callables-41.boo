import NUnit.Framework
	
a = { return "foo" }, { return "bar" }
Assert.AreEqual("foo", a[0]())
Assert.AreEqual("bar", a[-1]())
