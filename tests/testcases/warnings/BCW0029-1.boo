"""
BCW0029-1.boo(13,9): BCW0029: WARNING: Method 'C.B()' hides inherited non virtual method 'B.B()'. Declare 'C.B()' as a 'new' method. 
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
