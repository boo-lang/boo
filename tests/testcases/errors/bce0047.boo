"""
override0.boo(7,14): BCE0047: Method 'BooCompiler.Tests.DerivedClass.Method2()' cannot be overriden because it is not virtual.
"""
import BooCompiler.Tests from BooCompiler.Tests

class A(DerivedClass):
	override def Method2():
		pass
