"""
A.Foo.set
B.Foo.set
"""


class A:

	virtual Foo:
		get:
			return "A.Foo"
		set:
			print("A.Foo.set")
			
class B(A):

	override Foo:
		get:
			return "B.Foo"
			
		set:
			print("B.Foo.set")

a = A()
a.Foo = "foo"
assert "A.Foo" == a.Foo


a = B()
a.Foo = "foo"
assert "B.Foo" == a.Foo
			
