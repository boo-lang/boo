"""
foo bar
"""
class Test:
	public a as duck = array(string, 2)
	
t as duck = Test()
t.a[0] = "foo"
t.a[1] = "bar"
print join(t.a)
