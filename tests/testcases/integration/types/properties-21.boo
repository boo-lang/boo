"""
A.Foo.set
A.Foo.set
A.Foo.set
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
			return "B: ${super()}"

a = A()
a.Foo = "foo"
assert "A.Foo" == a.Foo

a = B()
a.Foo = "foo"
assert "B: A.Foo" == a.Foo

b = B()
b.Foo = "foo"
assert "B: A.Foo" == b.Foo
			
