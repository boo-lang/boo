"""
BCE0188-1.boo(7,7): BCE0188: The default for 'a' must be a constant.
"""
def side() as int:
	return 7

def f(a as int = side()):
	return a

# Metadata carries a constant, not an expression, so a call would silently get
# nothing rather than the value written here.
# This test tells us whether a default that is not constant is refused.
print f()
