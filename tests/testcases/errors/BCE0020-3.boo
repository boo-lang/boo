"""
BCE0020-3.boo(9,18): BCE0020: An instance of type 'Test' is required to access non static member 'F1'.
"""
class Test:

	def F1 ():
		pass

	static FA = [F1]
	FB = [F1]
