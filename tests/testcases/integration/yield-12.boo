"""
0
1
0, 8, 16, 24
"""
class Generator:
	
	
	[property(B)]
	_b = 5
	
	def run(begin as int, end as int):
		a = 0
		yield { return a }
		yield { --begin; return ++a }
		yield { a = _b; return { return (a+_b)*i for i in range(begin, end) } }
		assert 5 == a
	
g = Generator()
for f in g.run(1, 4):
	item = f()	
	if item isa callable:
		g.B = 3
		print(join(cast(callable, item)(), ", "))
	else:
		print(item)
