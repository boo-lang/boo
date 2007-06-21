"""
0
0
3
"""
class Value(System.ValueType):
	public Value as int
	
actual = array(Value, 2)
print actual[0].Value
print actual[1].Value

actual[0].Value = 3
print actual[0].Value
