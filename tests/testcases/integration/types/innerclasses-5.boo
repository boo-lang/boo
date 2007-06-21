class Outer:	

	[getter(Values)]
	_values as (Inner)

	class Inner:
		[property(Name)]
		_name
		
		def constructor(name):
			_name = name

	def constructor():
		_values = Inner("foo"), Inner("bar")

outer = Outer()
assert 2 == len(outer.Values)
assert "foo" == outer.Values[0].Name
assert "bar" == outer.Values[1].Name
		
