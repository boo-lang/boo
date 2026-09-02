"""
BCE0005-named-argument.boo(10,12): BCE0005: Unknown identifier: 'foo'.
"""
def greet(name as string, greeting as string = "Hi"):
	print "${greeting}, ${name}!"

# The right hand side is resolved like any other expression, so an unknown
# one is reported rather than swallowed by the named-argument handling.
# This test tells us whether a bad value is still diagnosed.
greet(name=foo)
