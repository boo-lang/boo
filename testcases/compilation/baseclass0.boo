"""
BaseClass.Method0
BaseClass.Method1
"""
using Boo.Tests.Ast.Compilation from Boo.Tests

class A(BaseClass):
	def constructor():
		Method0()
		
a = A()
a.Method1()
