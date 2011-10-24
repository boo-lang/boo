

i = 0
g = (++i)*j for j in range(3)

assert "0 2 6" == join(g)
assert 3 == i
