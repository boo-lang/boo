"""
0 2 4 6 8
"""
def even(value):	
	return 0 == cast(int, value) % 2

l = List(range(10))
print(join(l.Collect(even)))
