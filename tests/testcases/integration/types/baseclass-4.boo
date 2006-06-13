"""
BaseClass.constructor('Hello!')
BaseClass.Method0

"""
import BooCompiler.Tests from BooCompiler.Tests

class A(BaseClass):
	def constructor():
		super('Hello!')
		
b as BaseClass = A()
b.Method0()

