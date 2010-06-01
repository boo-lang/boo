"""
BCE0047-4.boo(9,9): BCE0047: Method 'A.B()' cannot be overridden because it is not virtual.
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


