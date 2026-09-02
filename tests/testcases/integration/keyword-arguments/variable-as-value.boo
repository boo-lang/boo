"""
Hi, Jude!
Hey, Jude!
Hey, Jude!
"""
def greet(name as string, greeting as string = "Hi"):
	print "${greeting}, ${name}!"

# The name on the left picks the parameter; the right is an ordinary
# expression, even when it happens to be a variable of the same name.
# This test tells us whether the two sides are read independently.
name = "Jude"
greeting = "Hey"
greet(name=name)
greet(name=name, greeting=greeting)
greet(greeting=greeting, name=name)
