

class MyHash(Hash):
	pass
	
h = MyHash()
h[3] = 4
assert 4 == h[3]
