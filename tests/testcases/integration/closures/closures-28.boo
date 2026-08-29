a = 0
f1 = def ():
	b = 1
	a = 5
	return { return a+b }
	
f = f1()
assert f is not null
assert 6 == f()
assert 5 == a
