"""
BCE0047-1.boo(7,18): BCE0047: Method 'BooCompiler.Tests.DerivedClass.Method2()' cannot be overridden because it is not virtual.
"""
import BooCompiler.Tests from BooCompiler.Tests

class A(DerivedClass):
	override def Method2():
		pass
