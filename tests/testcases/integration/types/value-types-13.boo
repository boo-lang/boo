"""
Value: 5
Value: 10
"""
class Value(System.ValueType):
	public Value as int
	
	def constructor(v as int):
		self.Value = v
		
	override def ToString():
		return "Value: ${Value}"
	
actual = (Value(5), Value(10))
o1 as object = actual[0]
o2 as object = actual[1]

print o1
print o2
