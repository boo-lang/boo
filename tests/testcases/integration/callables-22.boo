import NUnit.Framework

callable StringFunction() as string

def foo():
	return "foo"
   
a = foo
Assert.AreEqual("foo", a())

fn as StringFunction
fn = a
Assert.AreEqual("foo", fn())
