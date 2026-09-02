"""
BCE0187-1.boo(4,21): BCE0187: Parameter 'b' has no default, so it cannot follow one that has.
"""
def f(a as int = 1, b as int):
	return a + b

# A default only covers arguments the caller stops writing, so nothing after
# one can be required; f(1) would leave b with nothing to take.
# This test tells us whether a required parameter after an optional is refused.
print f(1, 2)
