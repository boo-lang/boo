"""
BCE0047-5.boo(13,9): BCE0047: Method 'B.B()' cannot be overridden because it is not virtual.
"""
class A:
	virtual def B():
		pass

class B(A):
	final def B():
		pass
		
class C(B):
	def B():
		pass
