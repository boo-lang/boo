

a = array(i*2 for i in range(3))
assert a.GetType() is typeof((int))

assert 2 == a[-1]-a[-2]
assert -2 == a[-2]-a[-1]
