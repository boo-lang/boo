"""
it works for explicit self
it works for implicit self
"""
class Foo:
	def Bar():
		self.Baz("explicit self")
		Baz("implicit self")

[extension] def Baz(foo as Foo, target):
	print "it works for ${target}"
	
Foo().Bar()
