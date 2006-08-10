import System

class MyAttribute(Attribute):
	[getter(T)]
	_t
	
	def constructor(t):
		_t = t

class K:
	pass

	
attributes = Attribute.GetCustomAttributes(typeof(MyAttribute).Assembly, MyAttribute)
assert 1 == len(attributes)
assert K is cast(MyAttribute, attributes[0]).T

[assembly: MyAttribute(typeof(K))]
