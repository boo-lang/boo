import NUnit.Framework

class A:

	virtual Foo:
		get:
			return "A.Foo"
			
class B(A):

	Foo:
		get:
			return "B.Foo"

a = A()
Assert.AreEqual("A.Foo", a.Foo)

a = B()
Assert.AreEqual("B.Foo", a.Foo)
			
