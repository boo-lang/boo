"""
0
1
2
3
"""
m = matrix(int, 2, 2)
value = 0
for i in range(2):
	for j in range(2):
		m[i, j] = value
		print m[i, j].ToString()
		++value
