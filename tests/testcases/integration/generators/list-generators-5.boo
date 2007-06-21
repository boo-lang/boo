i = 0
l = [++i for j in range(3)]
assert l == [1, 2, 3]
assert 3 == i
