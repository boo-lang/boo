"""
BCE0070-3.boo(9,16): BCE0070: Definition of 'foo.bar' depends on 'foo.a' whose type could not be resolved because of a cycle. Explicitly declare the type of either one to break the cycle.
BCE0070-3.boo(6,9): BCE0070: Definition of 'foo.a' depends on 'foo.bar' whose type could not be resolved because of a cycle. Explicitly declare the type of either one to break the cycle.
"""
class foo:
	a = bar()
	
	def bar():
		return a
