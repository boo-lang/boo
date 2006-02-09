"""
0
1
2
"""

import BooCompiler.Tests

class test(BaseClass):
	def doit():
		return ProtectedProperty++

t = test()
print t.doit()
print t.doit()
print t.doit()
