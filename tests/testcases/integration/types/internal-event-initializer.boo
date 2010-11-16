"""
Foo.Bar
"""
class Foo:
	internal event Bar as callable()
	
	def Trigger():
		Bar()
	
Foo(Bar: { print "Foo.Bar" }).Trigger()
