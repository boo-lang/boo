import System

def assertArray(expected, actual):
	assert expected.GetType() is actual.GetType()
	assert expected == actual

a = (1, 2, 3)
b = (4, 5, 6)

assertArray((1, 2, 3, 4, 5, 6), a+b)

c = ("foo",)
d = ("bar",)

assertArray(("foo", "bar"), c+d)



