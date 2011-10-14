

class A:
	virtual def Foo():
		return "A.Foo"
		
	virtual def Bar():
		return "A.Bar"
		
class B(A):
	override def Foo():
		return "B.Foo"
		
	override def Bar():
		return "B.Bar: ${super.Foo()}"
		
a as A = A()
b as A = B()

assert "A.Foo" == a.Foo()
assert "B.Foo" == b.Foo()
assert "A.Bar" == a.Bar()
assert "B.Bar: A.Foo" == b.Bar()
