"""
A.Foo.set
A.Foo.set
"""
import NUnit.Framework

class A:

	Foo:
		virtual get:
			return "A.Foo"
		set:
			print("A.Foo.set")
			
class B(A):

	Foo:
		override get:
			return "B.Foo"
			
		set:
			print("B.Foo.set")

a = A()
a.Foo = "foo"
Assert.AreEqual("A.Foo", a.Foo)


a = B()
a.Foo = "foo"
Assert.AreEqual("B.Foo", a.Foo)
			
