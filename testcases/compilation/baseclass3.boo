"""
BaseClass.constructor('Hello!')
BaseClass.Method0

"""
import Boo.Tests.Lang.Compiler from Boo.Tests

class A(BaseClass):
	def constructor():
		super('Hello!')
		
b as BaseClass = A()
b.Method0()

