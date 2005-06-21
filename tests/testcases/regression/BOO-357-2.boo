"""
Foo.constructor
spam
Foo.constructor
Bar.constructor
eggs
"""
class Foo:	
	def constructor():
		print "Foo.constructor"
		
	virtual def spam():
		print "spam"
		
class Bar(Foo):
	def constructor():
		print "Bar.constructor"
		
	override def spam():
		print "eggs"
		
def create(flag as bool):
	return Foo() if flag
	return Bar()
		
f = create(true).spam
f()
f = create(false).spam
f()
