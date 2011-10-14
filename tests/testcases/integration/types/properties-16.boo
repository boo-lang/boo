"""
A.Foo.set
B.Foo.set
A.Foo
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
			
		set:
			print("B.Foo.set")
			print(super.Foo)

a = A()
a.Foo = "foo"
assert "A.Foo" == a.Foo


a = B()
a.Foo = "foo"
assert "B: A.Foo" == a.Foo
			
