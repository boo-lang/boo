"""
Hey, Jude!
5
"""
def greet(name as string, greeting as string = "Hi"):
	print "${greeting}, ${name}!"

# Naming a parameter must not reserve that identifier in the caller. Without
# this, 'name' stays a string local afterwards and 'name = 5' fails to compile.
# This test tells us whether a named argument leaves the caller's names alone.
greet(name="Jude", greeting="Hey")
name = 5
print name
