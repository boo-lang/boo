"""
BCE0047-1.boo(7,18): BCE0047: Non virtual method 'BooCompiler.Tests.SupportingClasses.DerivedClass.Method2()' cannot be overridden.
"""
import BooCompiler.Tests.SupportingClasses from BooCompiler.Tests

class A(DerivedClass):
	override def Method2():
		pass
