import NUnit.Framework

def foo():
	return "foo"
	
def bar():
	return "bar"
	
for expected, fn as ICallable in zip(["foo", "bar"], [foo, bar]):
	Assert.AreEqual(expected, fn())
