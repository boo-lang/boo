"""
BCE0047-2.boo(9,18): BCE0047: Method 'A.B()' cannot be overridden because it is not virtual.
"""
class A:
	def B():
		pass

class B(A):
	override def B():
		pass

t as A = B()
t.B()
