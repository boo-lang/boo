"""
Hi, Jude!
Hey, Jude!
Hey, Jude!
Hi, Jude!
"""
def greet(name as string, greeting as string = "Hi"):
	print "${greeting}, ${name}!"

# A default declared in Boo, left out and supplied, positionally and by name.
# This test tells us whether Boo can declare an optional parameter of its own.
greet("Jude")
greet("Jude", "Hey")
greet(name="Jude", greeting="Hey")
greet(name="Jude")
