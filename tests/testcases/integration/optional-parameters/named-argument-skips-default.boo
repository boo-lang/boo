"""
a=1 b=9
a=1 b=2 c=9
a=7 b=2 c=9
"""
def two(a as int = 1, b as int = 2) as string:
	return "a=$a b=$b"

def three(a as int = 1, b as int = 2, c as int = 3) as string:
	return "a=$a b=$b c=$c"

# A named argument that skips an earlier default leaves a hole before it.
# This test tells us whether the hole gets its default and the name its value.
print two(b=9)
print three(c=9)
print three(7, c=9)
