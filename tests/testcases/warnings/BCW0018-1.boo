"""
BCW0018-1.boo(6,9): BCW0018: WARNING: Overriding 'object.Finalize()' is bad practice. You should use destructor syntax instead.
"""

class Test:
	def Finalize():
		print "not good"

	def Finalize(nonApplicable as int):
		pass

	def Finalize[of T]():
		print "non-applicable"

class Test2:
	def destructor():
		pass

