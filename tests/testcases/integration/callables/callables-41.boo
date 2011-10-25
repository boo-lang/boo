
	
a = { return "foo" }, { return "bar" }
assert "foo" == a[0]()
assert "bar" == a[-1]()
