class Outer:
	class Inner:
		pass
		
	[getter(Value)]
	_value as Inner
	
	def constructor(value as Inner):
		_value = value
		
value = Outer.Inner()
outer = Outer(value)
assert value is outer.Value
