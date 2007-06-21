"""
A.Foo.set
A.Foo.set
A.Foo.set
"""
import NUnit.Framework

class A:

	virtual Foo:
		get:
			return "A.Foo"
		set:
			print("A.Foo.set")
			
class B(A):

	override Foo:
		get:
			return "B: ${super()}"

a = A()
a.Foo = "foo"
Assert.AreEqual("A.Foo", a.Foo)

a = B()
a.Foo = "foo"
Assert.AreEqual("B: A.Foo", a.Foo)

b = B()
b.Foo = "foo"
Assert.AreEqual("B: A.Foo", b.Foo)
			
