"""
1 2 3
1 2 3
"""
a1 = array(object, (1, 2, 3))
a2 = array(int, a1)
assert a2 == (1, 2, 3)

print join(a1)
print join(a2)

