t = int
a = array(t, 3)
assert typeof((int)) is a.GetType()
assert 3 == len(a)

t = string
a = array(t, 2)
assert typeof((string)) is a.GetType()
assert 2 == len(a)
