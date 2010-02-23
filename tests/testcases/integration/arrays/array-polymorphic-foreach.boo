"""
Bar.Run
Bar.Run
"""
class Foo:
	pass

class Bar(Foo):
	virtual def Run():
		print("Bar.Run")

foos = (of Foo: Bar(), Bar())
for foo as Bar in foos:
	foo.Run()
