class Outer:

	class Inner:
		[property(Value)]
		_value = null

	[getter(InnerValue)]
	_innerValue = Inner(Value: "foo")

outer = Outer()
assert "foo" == outer.InnerValue.Value
		
