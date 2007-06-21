import NUnit.Framework

class Foo:

	public spam = "spam"
	
	public eggs = 2
	
f = Foo()
Assert.AreEqual("spam", f.spam)
Assert.AreEqual(2, f.eggs)
