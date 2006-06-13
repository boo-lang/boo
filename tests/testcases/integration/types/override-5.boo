import NUnit.Framework

class A:

	virtual def Foo():
		return "A.Foo"
		
		
class B(A):
	pass
	
class C(B):

	override def Foo():
		return "C.Foo"
		
class D(C):
	
	override def Foo():
		return "D.Foo"
		
a = A()
Assert.AreEqual("A.Foo", a.Foo())

a = C()
Assert.AreEqual("C.Foo", a.Foo())

a = D()
Assert.AreEqual("D.Foo", a.Foo())
