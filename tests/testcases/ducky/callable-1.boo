"""
Foo.bar
42
42
before field
42
before property
42
"""
class Foo:
	[getter(Function)]
	public function as duck
	
	def constructor(function):
		self.function = function
		
	def bar():
		print "Foo.bar"
		function()
		self.function()
		
def bar():
	print "42"
	
f = Foo(bar)
f.bar()
print "before field"
f.function()
print "before property"
f.Function()
		
		
