"""
A.Foo.set
A.Foo.set
C.Foo.set
"""
import NUnit.Framework

class A:

	virtual Foo:
		get:
			return "A.Foo"
		set:
			print("A.Foo.set")
			
class B(A):
	pass
	
class C(B):

	override Foo:
		get:
			return "C.Foo"
			
		set:
			print("C.Foo.set")

a = A()
a.Foo = "foo"
Assert.AreEqual("A.Foo", a.Foo)

a = B()
a.Foo = "foo"
Assert.AreEqual("A.Foo", a.Foo)

a = C()
a.Foo = "foo"
Assert.AreEqual("C.Foo", a.Foo)
			
