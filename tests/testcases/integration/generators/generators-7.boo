"""
0 2 4 6 8
0 3 6 9 12
"""
a = (
		i*2 for i in range(5),
		i*3 for i in range(5)
	)
	
i, j = a
print(join(i))
print(join(j))
