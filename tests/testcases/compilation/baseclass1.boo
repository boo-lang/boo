"""
A.Method0
BaseClass.Method1
A.Method2

"""
import BooCompiler.Tests from BooCompiler.Tests

class A(BaseClass):
	def Method0():
		print("A.Method0") #overriden method
		
	def Method2():
		print("A.Method2") #new method
		
b as BaseClass = A()
b.Method0()
b.Method1()

(b as A).Method2()

