import NUnit.Framework

def foo():
	return "it works!"
	
fn as object
fn = foo

Assert.AreSame(System.MulticastDelegate, fn.GetType().BaseType)
Assert.AreEqual("it works!", cast(ICallable, fn)())
