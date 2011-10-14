

foo = def ():
	return "foo"
	
bar = def ():
	return "bar"
	
a = foo, bar
assert "foo" == a[0]()
assert "bar" == a[-1]()
