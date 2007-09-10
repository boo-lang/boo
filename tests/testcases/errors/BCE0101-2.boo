"""
BCE0101-2.boo(9,9): BCE0101: The return type of a generator must be either 'System.Collections.IEnumerable' or 'object'.
"""
class A:
	virtual def SpawnBalls():
		pass

class B(A):
	def SpawnBalls():
		yield
