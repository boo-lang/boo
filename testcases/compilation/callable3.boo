"""
0 2 4 6 8
"""
def even(value):
	i as int = value
	return 0 == i % 2

l = List(range(10))
print(join(l.Collect(even)))
