"""
Extension.Bar
Foo2.Baz
Foo.Bar
"""
class Foo:
	protected def Bar():
		print "Foo.Bar"
		
class Foo2(Foo):
	def Baz():
		print "Foo2.Baz"
		Bar()
		
[Extension]
def Bar(f as Foo):
	print "Extension.Bar"
	
// if a method is not accessible at a specific call site
// but an extension method is, the extension method
// should be called
Foo2().Bar()
Foo2().Baz()
