"""
BCE0071-1.boo(10,9): BCE0071: Inheritance cycle detected: 'A'.
"""
class A(B):
	pass
	
class B(C):
	pass
	
class C(A):
	pass
	

