"""
spam
eggs
"""
class Foo:
	
	_function as callable()
	
	def constructor():
		_function = spam
		
	virtual def spam():
		print "spam"
		
	def run():
		_function()
		
class Bar(Foo):
	override def spam():
		print "eggs"
		
f = Foo()
b = Bar()
f.run()
b.run()
