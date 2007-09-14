"""
Extensions.Bar
Foo2.Baz
Foo.Bar
"""
import Extensions

class Foo:
	protected Bar:
		get:
			print "Foo.Bar"
			return null
		
class Foo2(Foo):
	Baz:
		get:
			print "Foo2.Baz"
			return Bar
		
class Extensions:
	[Extension]
	static Bar[f as Foo]:
		get:
			print "Extensions.Bar"
			return
	
a = Foo2().Bar
b = Foo2().Baz
