"""
BCE0101-2.boo(9,9): BCE0101: Return type 'void' cannot be used on a generator. Did you mean 'void*' ? Or use a 'System.Collections.IEnumerable' or 'object'.
"""
class A:
	virtual def SpawnBalls():
		pass

class B(A):
	def SpawnBalls():
		yield
