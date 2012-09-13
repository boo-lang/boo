

class A:

	virtual def Foo(a as int):
		return "A.Foo(${a})"

	virtual def Foo():
		return "A.Foo"
		
class B(A):
	
	def Foo(a as int):
		return "B.Foo(${a})"

	override def Foo():
		return "B.Foo"
		
a = A()
assert "A.Foo" == a.Foo()
assert "A.Foo(3)" == a.Foo(3)

a = B()
assert "B.Foo" == a.Foo()
assert "B.Foo(5)" == a.Foo(5)
