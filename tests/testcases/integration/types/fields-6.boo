import NUnit.Framework

class Foo:

	public static spam = i*eggs for i in range(3)
	
	public static eggs = 2
	
Assert.AreEqual("0 2 4", join(Foo.spam))
Assert.AreEqual(2, Foo.eggs)
Foo.eggs = 3
Assert.AreEqual("0 3 6", join(Foo.spam))
