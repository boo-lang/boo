"""
B.Name = 'Thelonius'
Thelonius
"""
class A:

	_name as string
	
	virtual Name:
		get:
			return _name
		set:
			_name = value
			
class B(A):
	override Name:
		set:
			print "B.Name = '${value}'"
			super(value)
			
	
b = B()
b.Name = "Thelonius"
print b.Name
