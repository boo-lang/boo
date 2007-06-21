import NUnit.Framework

def foo():
	return "foo"
	
def bar():
	return "bar"
	
i = -1
expected = "foo", "bar"
for f in foo, bar:
	Assert.AreEqual(expected[++i], f())
