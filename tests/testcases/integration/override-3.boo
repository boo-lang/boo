import NUnit.Framework

class A:

	virtual def Foo():
		return "A.Foo"
		
		
class B(A):

	override def Foo():
		return "B.Foo"
		
a = A()
Assert.AreEqual("A.Foo", a.Foo())

a = B()
Assert.AreEqual("B.Foo", a.Foo())
