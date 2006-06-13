import NUnit.Framework

class A:

	def Foo(a as int):
		return "A.Foo(${a})"

	virtual def Foo():
		return "A.Foo"
		
		
class B(A):
	
	def Foo(a as int):
		return "B.Foo(${a})"

	override def Foo():
		return "B.Foo"
		
a = A()
Assert.AreEqual("A.Foo", a.Foo())
Assert.AreEqual("A.Foo(3)", a.Foo(3))

a = B()
Assert.AreEqual("B.Foo", a.Foo())
Assert.AreEqual("A.Foo(5)", a.Foo(5))
Assert.AreEqual("B.Foo(2)", B().Foo(2))
