def odds(l):
	for i in l:
		yield i if 0 != i % 2
