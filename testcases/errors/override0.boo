"""
override0.boo(7,14): BCE0047: Method 'Boo.Tests.Lang.Compiler.DerivedClass.Method2()' cannot be overriden because it is not virtual.
"""
import Boo.Tests.Lang.Compiler from Boo.Tests

class A(DerivedClass):
	override def Method2():
		pass
