"""
11
16
"""
# The ref parameter itself takes no default; only a later, ordinary
# parameter can, and the call can still omit or supply it.
def foo(ref value as int, increment as int = 1):
	value += increment

x = 10
foo(x)
print x
foo(x, 5)
print x
