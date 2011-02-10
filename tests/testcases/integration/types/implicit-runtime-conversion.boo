"""
op_Implicit
42
"""
class ConvertibleFoo:

	static def op_Implicit(c as ConvertibleFoo) as Bar:
		print "op_Implicit"
		return Bar(Value: c.Value)
		
	public Value as int
	
class Bar:
	public Value = 0
	
o as object = ConvertibleFoo(Value: 42)
b as Bar = o
print b.Value
