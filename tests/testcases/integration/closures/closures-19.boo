

a = 2
b = do ():
	c = 4
	r as callable = { return a*c }
	w as callable = { value | c = value }
	return r, w
	
reader, writer = b()
assert 8 == reader()

writer(3)
assert 6 == reader()

a = 5
assert 15 == reader()

writer(2)
assert 10 == reader()


