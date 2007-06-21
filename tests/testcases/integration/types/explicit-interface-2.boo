"""
True
False
"""

interface IFoo:
	def Func() as bool:
		pass

interface IBar:
	def Func() as bool:
		pass

class FooBar(IFoo, IBar):
	def IFoo.Func():
		return true
	
	def IBar.Func():
		return false

fb = FooBar()
print cast(IFoo, fb).Func()
print cast(IBar, fb).Func()
