"""
4
"""
class Integer:
	_value as int
	
	def constructor(value as int):
		_value = value
	
	Value:
		get:
			return _value
		set:
			_value = value

	override def ToString():
		return _value.ToString()

i = Integer(3)
++i.Value
print(i)
