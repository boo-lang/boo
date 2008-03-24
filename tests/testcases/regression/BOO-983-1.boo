"""
FooBar.Func(as Bar)
Bar
FooBar(System.Object,IFoo`1[Bar])
"""

class FooBar(IFoo of Bar):
	def Func(arg as Bar):
		print "FooBar.Func(as Bar)"
		return Bar()

interface IFoo[of T]:
	def Func(arg as T) as T

class Bar:
	pass

foobar = FooBar()

print foobar.Func(Bar())
print "${typeof(FooBar)}(${typeof(FooBar).BaseType},${join(typeof(FooBar).GetInterfaces(),',')})"
