#category FailsOnMono
"""
Value: 5
Value: 10
Value: 50
Value: 100
"""
class Value(System.ValueType):
	public Value as int
	
	def constructor(v as int):
		self.Value = v
		
	override def ToString():
		return "Value: ${Value}"
	
actual = (
			(Value(5), Value(10)),
			(Value(50), Value(100))
		)
for v1, v2 in actual:
	print v1
	print v2
