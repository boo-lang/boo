"""
override0.boo(7,14): BCE0047: Method 'Boo.Lang.Compiler.Tests.DerivedClass.Method2()' cannot be overriden because it is not virtual.
"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

class A(DerivedClass):
	override def Method2():
		pass
