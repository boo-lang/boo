"""
before
evaluated
after
before
evaluated
evaluated
after
"""
def fun(value):
	print('evaluated')
	return value
	
a = null and true
assert a is null

b = true and 3
assert int is b.GetType()
assert 3 == b

print("before")
c = fun(false) and fun(true)
print("after")
assert not c

print("before")
d = fun(true) and fun(null)
print("after")
assert d is null

e = 0 and false
assert 0 == e
