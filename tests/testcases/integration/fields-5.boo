import NUnit.Framework

class Foo:

	public spam = i*eggs for i in range(3)
	
	public eggs = 2
	
f = Foo()

Assert.AreEqual("0 2 4", join(f.spam))
Assert.AreEqual(2, f.eggs)
f.eggs = 3
Assert.AreEqual("0 3 6", join(f.spam))
