"""
BCE0184-1.boo(11,6): BCE0184: Argument 'name' is given more than once.
BCE0184-1.boo(12,6): BCE0184: Argument 'name' is given more than once.
"""
def greet(name as string, greeting as string = "Hi"):
	print "${greeting}, ${name}!"

# A parameter filled twice, once by two names and once by a positional
# argument the name then collides with.
# This test tells us whether a place claimed twice is refused.
greet(name="a", name="b")
greet("Jude", name="dup")
