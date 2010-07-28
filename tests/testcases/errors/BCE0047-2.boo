"""
BCE0047-2.boo(9,18): BCE0047: Non virtual method 'A.B()' cannot be overridden.
"""
class A:
	def B():
		pass

class B(A):
	override def B():
		pass

t as A = B()
t.B()
