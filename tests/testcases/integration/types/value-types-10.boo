"""
5
10
"""
class Value(System.ValueType):
	public Value as int
	
	def constructor(v as int):
		self.Value = v
	
actual = (Value(5), Value(10))
print actual[0].Value
print actual[1].Value
