"""
2
"""
class Foo:
	[property(Value)]
	_value = 0
	
class Bar:
	[property(F)]
	_f = Foo()
	
class Baz:
	[getter(B)]
	_bar = Bar()
	def go():
		_bar.F.Value += 2
		yield _bar.F.Value
		
b = Baz()
for item in b.go():
	print item
	
