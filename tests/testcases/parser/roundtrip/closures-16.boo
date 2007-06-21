"""
def gti(value as int):
	return { arg as int | return true if (arg > value) }

f = gti(5)
assert f(6)
assert not f(3)
"""
def gti(value as int):
	return { arg as int | return true if arg > value }

f = gti(5)
assert f(6)
assert not f(3)
