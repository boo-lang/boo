"""
Hey, Jude!
Hey, Jude!
Hey, Jude!
"""
def greet(name as string, greeting as string):
	print "${greeting}, ${name}!"

# Order is irrelevant once an argument names its parameter. Written positionally
# the third call would read greet("Hey", "Jude") and come out backwards.
# This test tells us whether a named argument reaches the parameter it names.
greet("Jude", "Hey")
greet(name="Jude", greeting="Hey")
greet(greeting="Hey", name="Jude")
