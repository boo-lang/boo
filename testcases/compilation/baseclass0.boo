"""
overriden method
base method
new method

"""
using Boo.Tests.Ast.Compilation

class A(BaseClass):
	def Method0():
		print("overriden method")
		
	def Method2():
		print("new method")
		
b as BaseClass = A()
b.Method0()
b.Method1()

(b as A).Method2()

