"""
A.constructor
A.constructor
B.constructor
C.constructor
A.constructor
A.constructor
B.constructor
D.constructor
"""
class A:
	def constructor():
		print("A.constructor")
		
class B(A):
	def constructor():
		super()
		print("B.constructor")
		
class C(A):
	def constructor():
		print("C.constructor")
		super()
		
class D(B):
	def constructor():
		print("D.constructor")
		
A()
B()
C()
D()
