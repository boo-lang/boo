import NUnit.Framework

class A:

	virtual def Foo():
		return "A.Foo"
		
		
class B(A):
	pass
	
class C(B):

	override def Foo():
		return "C.Foo"
		
a = A()
Assert.AreEqual("A.Foo", a.Foo())

a = C()
Assert.AreEqual("C.Foo", a.Foo())
