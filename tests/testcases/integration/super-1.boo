import NUnit.Framework

class A:
	virtual def Foo():
		return "A"
		
class B(A):
	override def Foo():
		return super() + "B"
		
class C(B):
	override def Foo():
		return super() + "C"
		

Assert.AreEqual("A", A().Foo())
Assert.AreEqual("AB", B().Foo())
Assert.AreEqual("ABC", C().Foo())
