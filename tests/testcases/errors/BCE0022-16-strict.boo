"""
BCE0022-16-strict.boo(11,12): BCE0022: Cannot convert 'Foo' to 'Bar'.
"""
class Foo:
	pass
	
class Bar(Foo):
	pass
	
f = Foo()
b as Bar = f
c = f cast Bar
d = f as Bar

[assembly: StrictMode]
