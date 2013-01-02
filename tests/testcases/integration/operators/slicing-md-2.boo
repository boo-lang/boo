"""
2
3
3
0
1
2
10
4
5
20
7
8
"""

# basic multi-dimensional slicing assignment
a = matrix(int,3,3)
for i in range(0,3):
	for j in range(0,3):
		a[i,j] = i + 3*j

b = matrix(int,2)
b[0] = 10
b[1] = 20

a[0,1:3] = b

print a.Rank
print a.GetLength(0)
print a.GetLength(1)

print a[0,0]
print a[1,0]
print a[2,0]
print a[0,1]
print a[1,1]
print a[2,1]
print a[0,2]
print a[1,2]
print a[2,2]

