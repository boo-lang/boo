

class A:

	virtual def Foo():
		return "A.Foo"
		
		
class B(A):

	override def Foo():
		return "B.Foo"
		
a = A()
assert "A.Foo" == a.Foo()

a = B()
assert "B.Foo" == a.Foo()
