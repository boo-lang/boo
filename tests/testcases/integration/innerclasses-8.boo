import NUnit.Framework

class Outer:
	class Inner:
		pass
		
	[getter(Value)]
	_value as Inner
	
	def constructor(value as Inner):
		_value = value
		
value = Outer.Inner()
outer = Outer(value)
Assert.AreSame(value, outer.Value)
