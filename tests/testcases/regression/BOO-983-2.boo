"""
Foo`1[Bar].Func(as Bar)
Bar
FooBar(Foo`1[Bar],)
"""

class FooBar(Foo of Bar):
	pass

class Foo[of T]:
	def Func(arg as T):
		print "Foo`1[${typeof(T)}].Func(as ${typeof(T)})"
		return arg

class Bar:
	pass

foobar = FooBar()

print foobar.Func(Bar())
print "${typeof(FooBar)}(${typeof(FooBar).BaseType},${join(typeof(FooBar).GetInterfaces(),',')})"
