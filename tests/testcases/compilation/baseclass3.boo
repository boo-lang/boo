"""
BaseClass.constructor('Hello!')
BaseClass.Method0

"""
import Boo.Lang.Compiler.Tests from Boo.Lang.Compiler.Tests

class A(BaseClass):
	def constructor():
		super('Hello!')
		
b as BaseClass = A()
b.Method0()

