"""
1 2
2 3
1


1 2 3
"""
a = 1, 2, 3
print(join(a[-4:-1]))
print(join(a[1:10]))
print(join(a[-10:-2]))
print(join(a[-10:-5]))
print(join(a[-10:-20]))
print(join(a[:]))

