"""
BCE0090-1.boo(9,28): BCE0090: Derived method 'B.Foo' can not reduce the accessibility of 'A.Foo' from 'public' to 'protected'.
"""
class A:
	virtual def Foo():
		pass
		
class B(A):
	override protected def Foo():
		pass

