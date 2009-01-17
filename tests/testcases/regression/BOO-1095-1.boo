"""
Bar
"""

class Bar:
	pass

interface IFoo:
	def Bar(ref x as Bar) as bool

class Foo (IFoo):
	def Bar(ref x as Bar) as bool:
		print "Bar"
		return true

x as Bar
Foo().Bar(x) #was: BCE0004: Ambiguous reference 'Bar': Foo.Bar(Bar), Foo.Bar(Bar), Foo.Bar(Bar).

