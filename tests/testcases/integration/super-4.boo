import NUnit.Framework

class A:
	def constructor():
		Assert.AreSame(self, super)		
		
class B(A):
	def constructor():
		Assert.AreSame(self, super)
		
A()
B()


