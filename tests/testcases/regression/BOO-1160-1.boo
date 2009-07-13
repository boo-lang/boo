"""
42
"""
class Test:
	_value as double?

	Value as double: #not a nullable
		get: return _value

	def constructor(value as double):
		_value = value

t = Test(42)
print t.Value

