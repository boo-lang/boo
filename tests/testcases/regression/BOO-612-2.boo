"""
4
"""
def foo(ref x as int):
	a = x = 4
	return a
	
x = 0
print foo(x)
assert 4 == x
