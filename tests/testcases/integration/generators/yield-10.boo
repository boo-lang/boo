"""
0
1
6, 3
"""
b = 5
g = def ():
	a = 0
	yield { return a }
	yield { return ++a }
	yield { a = b; return { yield ++a; yield b } }
	assert 6 == a
	
	
for f in g():
	item = f()	
	if item isa callable:
		b = 3
		print(join(cast(callable, item)(), ", "))
	else:
		print(item)
