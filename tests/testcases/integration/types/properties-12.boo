

class A:

	virtual Foo:
		get:
			return "A.Foo"
			
class B(A):

	Foo:
		override get:
			return "B.Foo"

a = A()
assert "A.Foo" == a.Foo

a = B()
assert "B.Foo" == a.Foo
			
