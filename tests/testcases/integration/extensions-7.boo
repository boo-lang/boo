"""
Foo.Bar
Extension.Bar
"""
class Foo:
	def Bar():
		print "Foo.Bar"
		
def Bar(self as Foo, i as int):
	print "Extension.Bar"
	
Foo().Bar()
Foo().Bar(42)
