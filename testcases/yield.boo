def odds(l):
	for i in l:
		yield i if 0 != i % 2
		
def double(i):
	return i*2
	
def map(fn, iterator):
	for item in iterator:
		yield fn(item)
		
for odd in map(double, odds([1, 2, 3, 4, 5])):
	print(odd)
