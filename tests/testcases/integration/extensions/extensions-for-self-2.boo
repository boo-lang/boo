"""
it works for explicit self
it works for implicit self
no arguments
no arguments
"""
class Foo:
	def Bar():
		self.Baz("explicit self")
		Baz("implicit self")
		self.Baz()
		Baz()

[extension] def Baz(foo as Foo, target):
	print "it works for ${target}"
	
[extension] def Baz(foo as Foo):
	print "no arguments"
	
Foo().Bar()
