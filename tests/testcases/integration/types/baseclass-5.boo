"""
A.Method0
BaseClass.Method1
"""

import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class A(DerivedClass):
	def Method0():
		print("A.Method0") #overriden method	
	
		
a = A()
a.Method2() # see DerivedClass.Method2 for details
