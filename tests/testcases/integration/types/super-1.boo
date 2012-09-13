

class A:
	virtual def Foo():
		return "A"
		
class B(A):
	override def Foo():
		return super() + "B"
		
class C(B):
	override def Foo():
		return super() + "C"
		

assert "A" == A().Foo()
assert "AB" == B().Foo()
assert "ABC" == C().Foo()
