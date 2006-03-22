"""
Holder(3)
Holder(4)
Holder(5)
Holder(6)
"""

class Holder:
	def constructor(value as double):
		_value = value
	
	def op_Implicit(value as double) as Holder:
		return Holder(value)
		
	override def ToString():
		return "Holder(${_value})"
	
	def op_Addition(left as Holder, right as Holder):
		return Holder(left._value + right._value)
		
	[getter(Value)] _value as double

def testreturn() as Holder:
	return 6.0
	
print Holder.op_Addition(Holder(1), 2.0)

print Holder(1) + 3.0
print 3.0 + Holder(2)

print testreturn()

h = Holder(5)
h = h + 1.0
h += 2.0
h += Holder(1)
//++h


