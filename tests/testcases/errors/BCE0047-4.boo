"""
BCE0047-4.boo(9,9): BCE0047: Non virtual method 'A.B()' cannot be overridden.
"""
class A:
	def B():
		pass

class B(A):
	def B():
		pass

t as A = B()
t.B()

		
[assembly: StrictMode]


