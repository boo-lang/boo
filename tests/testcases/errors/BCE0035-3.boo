"""
BCE0035-3.boo(5,9): BCE0035: 'A.Dispose' conflicts with inherited member 'System.IDisposable.Dispose'.
"""
class A(System.IDisposable):
	def System.IDisposable.Dispose() as object:
		pass
