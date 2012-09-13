
a = false or 5
b = true or 3

assert int is a.GetType()
assert int is b.GetType()
assert 5 == a*b
assert 5 == a
assert 1 == b
assert 0 == (false and 3)

