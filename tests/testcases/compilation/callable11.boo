import NUnit.Framework

def foo():
	return "yes, it works!"
	
Assert.AreEqual("yes, it works!", foo.Invoke())
	
	
