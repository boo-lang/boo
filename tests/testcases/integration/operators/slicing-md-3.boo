"""
2
3
6
"""

# basic multi-dimensional slicing
a = matrix(int,3,3)
for i in range(0,3):
	for j in range(0,3):
		a[i,j] = i + 3*j

b = a[0,1:]
print b.Length
print b[0]
print b[1]

