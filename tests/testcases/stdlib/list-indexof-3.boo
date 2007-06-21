a = [(1, 2), (3, 4), (5, 6)]

assert 0 == a.IndexOf((1, 2))
assert 1 == a.IndexOf((3, 4))
assert -1 == a.IndexOf((2, 1))
assert 2 == a.IndexOf((5, 6))
