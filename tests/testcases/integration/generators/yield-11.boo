"""
0
1
0, 8, 16
"""
b = 5
g = def ():
	a = 0
	yield { return a }
	yield { return ++a }
	yield { a = b; return { return (a+b)*i for i in range(3) } }
	assert 5 == a
	
	
for f in g():
	item = f()	
	if item isa callable:
		b = 3
		print(join(cast(callable, item)(), ", "))
	else:
		print(item)
