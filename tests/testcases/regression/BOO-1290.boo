"""
0
"""
interface C:
	def fun() as void

class D[of T(struct)](C):
	def x() as T:
		t as T
		return t
	def C.fun() as void:
		print x()
 
def doIt(c as C):
	c.fun()
	
doIt D[of int]()
		

