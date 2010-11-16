"""
42
"""
class Foo:
	internal Value:
		get: return _value
		set: _value = value
	_value = 0
	
print Foo(Value: 42).Value
