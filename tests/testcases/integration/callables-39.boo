import NUnit.Framework

def foo():
	return "foo"
	
def bar():
	return "bar"
	
a = foo, bar
Assert.AreEqual("foo", a[0]())
Assert.AreEqual("bar", a[-1]())
