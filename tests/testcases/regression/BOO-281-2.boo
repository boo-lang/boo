"""
21
"""
m = matrix(int, 2, 2)
value = 0
for i in range(2):
	for j in range(2):
		m[i, j] = ++value
		
value = (m[0, 0] + m[0, 1]) * (m[1, 0] + m[1, 1])
print value
	
