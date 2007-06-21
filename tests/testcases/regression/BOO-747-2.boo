"""
foo bar
"""
class Test:
	_a = array(string, 2)
	
	a[index as int]:
		get:
			return _a[index]
		set:
			_a[index] = value
			
	override def ToString():
		return join(_a)
	
t as duck = Test()
t.a[0] = "foo"
t.a[1] = "bar"
print t
