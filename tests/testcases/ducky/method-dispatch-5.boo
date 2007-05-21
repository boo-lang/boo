"""
double: 42
int: 1
"""
class DoubleWrapper:
	static def op_Implicit(w as DoubleWrapper):
		return w.Value
		
	[getter(Value)]
	_value as double
	
	def constructor(value as double):
		_value = value
		
class Foo:
	def bar(i as int):
		print "int:", i
	def bar(d as double):
		print "double:", d
		
d as duck = Foo()
d.bar(DoubleWrapper(42.0))
d.bar(1)

