"""
def abs(i as int):
	return { return i } if (i > 0)
	return { return (-i) }

assert 3 == abs(-3)()
assert 1 == abs(1)()
"""
def abs(i as int):
	return { return i } if i > 0
	return { return -i }

assert 3 == abs(-3)()
assert 1 == abs(1)()
