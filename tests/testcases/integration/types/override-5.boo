

class A:

	virtual def Foo():
		return "A.Foo"
		
		
class B(A):
	pass
	
class C(B):

	override def Foo():
		return "C.Foo"
		
class D(C):
	
	override def Foo():
		return "D.Foo"
		
a = A()
assert "A.Foo" == a.Foo()

a = C()
assert "C.Foo" == a.Foo()

a = D()
assert "D.Foo" == a.Foo()
