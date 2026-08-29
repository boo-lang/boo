"""
0 1
1 2
3 4
1 2 3
True
True
0 1 2 3 4
"""
a = 0, 1, 2, 3, 4
print(join(a[0:2]))
print(join(a[1:3]))
print(join(a[3:]))
print(join(a[1:-1]))
b = a[:]
print(len(a) == len(b))
print(a is not b)
print(join(b))
