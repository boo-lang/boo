"""
5
10
"""
class Value(System.ValueType):
	public Value as int
	
	def constructor(v as int):
		self.Value = v
	
v1 = Value(5)
v2 = Value(10)

actual = array(Value, 2)
actual[0] = v1
actual[1] = v2

print actual[0].Value
print actual[1].Value
