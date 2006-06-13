import NUnit.Framework

foo = def ():
	return "foo"
	
bar = def ():
	return "bar"
	
a = foo, bar
Assert.AreEqual("foo", a[0]())
Assert.AreEqual("bar", a[-1]())
