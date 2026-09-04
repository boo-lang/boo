"""
BCE0004-optional-overload.boo(12,8): BCE0004: Ambiguous reference 'f': BCE0004_optional_overloadModule.f(int, int, int), BCE0004_optional_overloadModule.f(int, int).
"""
def f(a as int, b as int = 2):
	return "two"

def f(a as int, b as int = 2, c as int = 3):
	return "three"

# Needing a default filled does not rank one overload above the other.
# This test tells us whether the call is rejected instead of resolved.
print f(1)
