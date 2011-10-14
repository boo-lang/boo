"""
A.Foo.set
A.Foo.set
"""


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

a = A()
a.Foo = "foo"
assert "A.Foo" == a.Foo


a = B()
a.Foo = "foo"
assert "B.Foo" == a.Foo
			
