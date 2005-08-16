"""
0 0 1 0
0 0 2 0
0 0 3 0
"""
a = array(int, 4)
index = 2
a[index] += 1
print join(a)
++a[index]
print join(a)
a[index] = a[index] + 1
print join(a)
