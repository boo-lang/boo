a1 = (1, 2, 3)
s1 = "Steppenwolf"
l1 = [5, 6]
a2 = matrix(int, 3, 2)

assert 3 == len(a1)
assert 11 == len(s1)
assert 2 == len(l1)
assert 3 == len(a2, 0)
assert 2 == len(a2, 1)
