"""
2 4 4
"""
a = (1, 2, 3)
assert 2 == ++a[0]
assert 3 == ++a[1]
assert 4 == ++a[1]
assert 4 == ++a[-1]
print join(a)

