

def foo():
	return "foo"
	
def bar():
	return "bar"
	
a = foo, bar
assert "foo" == a[0]()
assert "bar" == a[-1]()
