"""
True
False
"""

interface IFoo:
	Prop as bool:
		get:
			pass

interface IBar:
	Prop as bool:
		get:
			pass

class FooBar(IFoo, IBar):
	IFoo.Prop:
		get:
			return true
	
	IBar.Prop:
		get:
			return false

fb = FooBar()
print cast(IFoo, fb).Prop
print cast(IBar, fb).Prop
