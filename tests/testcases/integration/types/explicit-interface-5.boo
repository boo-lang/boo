"""
7
generic
"""

interface IFoo[of T]:
	def Get() as T

interface IBar:
	def Get() as string

class FooBar(IFoo[of int], IBar):
	def IFoo[of int].Get() as int:
		return 7

	def IBar.Get() as string:
		return "generic"

fb = FooBar()
print cast(IFoo[of int], fb).Get()
print cast(IBar, fb).Get()
