a = 1
b = 2

a, b = b, a
assert 1 == b
assert 2 == a
