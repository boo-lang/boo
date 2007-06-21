"""
5
10
"""
class Value(System.ValueType):
	public Value as int
	
	def constructor(v as int):
		self.Value = v
	
actual = (Value(5), Value(10))
v1 = actual[0]
v2 = actual[1]

print v1.Value
print v2.Value
