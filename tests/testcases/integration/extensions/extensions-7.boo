"""
Foo.Bar
Extension.Bar
"""
class Foo:
	def Bar():
		print "Foo.Bar"
		
[Extension]
def Bar(f as Foo, i as int):
	print "Extension.Bar"
	
Foo().Bar()
Foo().Bar(42)
