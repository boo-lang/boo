"""
MyInt(3)
4
MyInt(5)
"""

struct MyInt:
	_v as int
	
	def constructor(v as MyInt):
		_v = v._v
		
	def constructor(v as int):
		_v = v
		
	def op_Implicit(value as int) as MyInt:
		return MyInt(value)
		
	self[item as MyInt] as int:
		get:
			return item._v
			
	override def ToString():
		return "MyInt(${_v})"


v as MyInt = 3
print v
v2 = MyInt(1)
print v2[4]
v3 = MyInt(MyInt(5))
print v3

