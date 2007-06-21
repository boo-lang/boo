import NUnit.Framework

class A:
	virtual def Foo():
		return "A.Foo"
		
	virtual def Bar():
		return "A.Bar"
		
class B(A):
	override def Foo():
		return "B.Foo"
		
	override def Bar():
		return "B.Bar: ${super.Foo()}"
		
a as A = A()
b as A = B()

Assert.AreEqual("A.Foo", a.Foo())
Assert.AreEqual("B.Foo", b.Foo())
Assert.AreEqual("A.Bar", a.Bar())
Assert.AreEqual("B.Bar: A.Foo", b.Bar())
