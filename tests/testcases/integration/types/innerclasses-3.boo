class Outer:

	class Inner:
		[property(Value)]
		_value = null
		
	[property(InnerValue)]
	_innerValue as Inner

value = Outer.Inner(Value: "foo")
outer = Outer(InnerValue: value)
assert value is outer.InnerValue
		
