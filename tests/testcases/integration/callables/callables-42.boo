
	
a = { return "foo" }, { return 3 }
assert "foo" == a[0]()
assert 3 == a[-1]()
