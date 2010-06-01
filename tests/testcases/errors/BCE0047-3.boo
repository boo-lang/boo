"""
BCE0047-3.boo(7,9): BCE0047: Method 'BooCompiler.Tests.SupportingClasses.DerivedClass.Method2()' cannot be overridden because it is not virtual.
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class A(DerivedClass):
	def Method2():
		pass
		
[assembly: StrictMode]
